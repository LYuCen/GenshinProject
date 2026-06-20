using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Concurrent;
using Connect;
using Connect.Converter;
using Connect.Protocol;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BuffSystem.ElementBuff.StatusEffect;

namespace Server
{
    public class UnityServerManager : MonoBehaviour
    {
        public static UnityServerManager Instance; // 单例方便全局访问
        public GameObject serverPlayerPrefab;

        private Socket _listener;
        private Dictionary<int, ServerPlayerObject> _players = new Dictionary<int, ServerPlayerObject>();
        private Dictionary<Socket, int> _socketToId = new Dictionary<Socket, int>();
        private int _nextId = 1000;

        // 主线程任务队列 
        private ConcurrentQueue<Action> _mainThreadTasks = new ConcurrentQueue<Action>();

        private Dictionary<int, float> _lastHeartbeatTime = new Dictionary<int, float>();
        private const float HeartbeatTimeout = 10f;

        // 客户端时间偏移量
        public Dictionary<int, float> TimeOffset = new();
        private struct ElementSyncState
        {
            public bool Enable;
            public float AttachTime; // 攻击时刻
        }
        private Dictionary<int, ElementSyncState[]> _prevElementState = new();

        void Awake() => Instance = this;

        void Start()
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, 8888));
            _listener.Listen(10);
            Debug.Log("<color=green>Unity Dedicated Server Started on port 8888...</color>");

            // 异步接收玩家连接
            _listener.BeginAccept(OnAccept, null);

            // 服务器广播 20 tick服务器
            InvokeRepeating(nameof(BroadcastWorldState), 0.05f, 0.05f);
            InvokeRepeating(nameof(TickElementBuffs), 0.02f, 0.02f);
        }

        void Update()
        {
            // 每帧在主线程处理来自 Socket 线程的任务
            while (_mainThreadTasks.TryDequeue(out var task))
            {
                task.Invoke();
            }
            CheckHeartbeatTimeout();
        }

        /// <summary>
        /// 玩家连接
        /// </summary>
        /// <param name="ar"></param>
        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket client = _listener.EndAccept(ar);
                client.NoDelay = true; // 禁用 Nagle 算法，解决 TCP 延迟合并包的问题
                client.SendBufferSize = 65536; // 调大发送缓冲区
                client.ReceiveBufferSize = 65536; // 调大接收缓冲区
                
                _mainThreadTasks.Enqueue(() =>
                {
                    int id = _nextId++;
                    _lastHeartbeatTime[id] = Time.time;
                    
                    GameObject go = Instantiate(serverPlayerPrefab);
                    ServerPlayerObject spo = go.GetComponent<ServerPlayerObject>();
                    spo.Init(id, client);

                    _players.Add(id, spo);
                    _socketToId.Add(client, id);

                    // 订阅 Buff 变更回调，实时广播给客户端
                    var ctrl = spo.GetComponent<ElementBuffController>();
                    ctrl.OnEffectAdded += (type) =>
                    {
                        int effectType = EffectTypeToInt(type);
                        Broadcast(ElementBuffConverter.PackStatusEffect(id, effectType, true, 0));
                    };
                    ctrl.OnEffectRemoved += (type) =>
                    {
                        int effectType = EffectTypeToInt(type);
                        Broadcast(ElementBuffConverter.PackStatusEffect(id, effectType, false, 0));
                    };

                    // 发送二进制 ID
                    SendToClient(client, new Message(MessageType.S2C_AllocId, BitConverter.GetBytes(id)));

                    // 通知所有在线玩家，新玩家加入
                    byte[] spawnData = PlayerDataConverter.PackSinglePlayer(spo.State);
                    BroadcastExcept(new Message(MessageType.S2C_PlayerSpawn, spawnData), id);

                    // 通知新玩家：所有现有玩家信息
                    byte[] allInfo = PlayerDataConverter.PackAllPlayers(_players);
                    SendToClient(client, new Message(MessageType.S2C_AllPlayerInfo, allInfo));

                    Debug.Log($"玩家{id}加入服务器");
                });

                _listener.BeginAccept(OnAccept, null);
            }
            catch (Exception ex)
            {
                Debug.LogError("Accept Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 收集全场数据并广播给所有人 20tick 一次
        /// </summary>
        private void BroadcastWorldState()
        {
            if (_players.Count == 0) return;

            byte[] data = PlayerDataConverter.PackPositions(_players); 
            Broadcast(new Message(MessageType.S2C_Position, data));
        }
        
        void TickElementBuffs()
        {
            foreach (var kv in _players)
            {
                var ctrl = kv.Value.GetComponent<ElementBuffController>();
                if (!ctrl) continue;

                if (!_prevElementState.TryGetValue(kv.Key, out var prevState))
                {
                    prevState = new ElementSyncState[8];
                    _prevElementState[kv.Key] = prevState;
                }

                for (int i = 0; i < 8; i++)
                {
                    var e = ctrl.Elements[i];
                    bool nowEnabled = e.Enable;
                    float nowAttachTime = e.AttachTime;

                    if (!prevState[i].Enable && nowEnabled)
                    {
                        Broadcast(ElementBuffConverter.PackAttach(kv.Key, i, e.Level, (int)e.GetVal(Time.time), e.Capacity));
                    }
                    else if (prevState[i].Enable && nowEnabled && !Mathf.Approximately(prevState[i].AttachTime, nowAttachTime))
                    {
                        Broadcast(ElementBuffConverter.PackAttach(kv.Key, i, e.Level, (int)e.GetVal(Time.time), e.Capacity));
                    }
                    else if (prevState[i].Enable && !nowEnabled)
                    {
                        Broadcast(ElementBuffConverter.PackDetach(kv.Key, i));
                    }

                    prevState[i] = new ElementSyncState { Enable = nowEnabled, AttachTime = nowAttachTime };
                }
            }
        }


        /// <summary>
        /// 发送消息给特定客户端
        /// </summary>
        /// <param name="s">套接字</param>
        /// <param name="m">通知</param>
        public void SendToClient(Socket s, Message m)
        {
            try
            {
                if (s == null || !s.Connected) return;

                // 2. 准备数据内容
                byte[] dataBytes = m.data;

                // 包体大小 = 2字节(type) + 数据长度
                int bodySize = 2 + dataBytes.Length;
                // 总长度 = 4字节(全包长度头) + 包体大小
                byte[] fullPacket = new byte[4 + bodySize];

                // 写入全包长度
                Buffer.BlockCopy(BitConverter.GetBytes(bodySize), 0, fullPacket, 0, 4);
                // 写入 Type 长度
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)m.type), 0, fullPacket, 4, 2);
                // 写入 FlatBuffers 字符串内容
                Buffer.BlockCopy(dataBytes, 0, fullPacket, 6, dataBytes.Length);

                s.BeginSend(fullPacket, 0, fullPacket.Length, SocketFlags.None, (ar) =>
                {
                    try
                    {
                        s.EndSend(ar);
                    }
                    catch
                    {
                        /* 处理掉线 */
                    }
                }, null);
            }
            catch
            {
                CloseClient(s);
            }
        }

        /// <summary>
        /// 清理断开客户端
        /// </summary>
        /// <param name="sclient"></param>
        public void CloseClient(Socket sclient)
        {
            // 同样需要确保在主线程销毁物体
            _mainThreadTasks.Enqueue(() =>
            {
                if (_socketToId.TryGetValue(sclient, out int playerId))
                {
                    if (_players.TryGetValue(playerId, out var spo))
                    {
                        Destroy(spo.gameObject); // 销毁物理假人
                        _players.Remove(playerId);
                        _prevElementState.Remove(playerId);
                    }

                    _socketToId.Remove(sclient);
                    
                    Debug.Log($"客户端 {playerId} 断开连接，已清理物体.");
                    byte[] exitIdBytes = BitConverter.GetBytes(playerId);
                    Broadcast(new Message(MessageType.S2C_PlayerExit, exitIdBytes));
                    _lastHeartbeatTime.Remove(playerId);
                }

                try
                {
                    if (sclient.Connected) sclient.Shutdown(SocketShutdown.Both);
                    sclient.Close();
                }
                catch (Exception)
                {
                    // ignored
                }
                
            });
        }

        /// <summary>
        /// 将消息发送给所有在线玩家
        /// </summary>
        /// <param name="msg"></param>
        public void Broadcast(Message msg)
        {
            // 复制一份 Socket 列表避免多线程冲突
            List<Socket> targets = new List<Socket>(_socketToId.Keys);
            foreach (var sc in targets)
            {
                SendToClient(sc, msg);
            }
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="task"></param>
        public void EnqueueTask(Action task)
        {
            if (task != null)
            {
                _mainThreadTasks.Enqueue(task);
            }
        }

        private void CheckHeartbeatTimeout()
        {
            float now = Time.time;
            List<int> timeoutPlayers = new List<int>();

            foreach (var kv in _lastHeartbeatTime)
            {
                if (now - kv.Value > HeartbeatTimeout)
                {
                    timeoutPlayers.Add(kv.Key);
                }
            }

            foreach (var id in timeoutPlayers)
            {
                Debug.Log($"玩家 {id} 心跳超时，断开连接");

                if (_players.TryGetValue(id, out var spo))
                {
                    CloseClient(spo.ClientSocket);
                }
            }
        }

        public ServerPlayerObject GetPlayer(int id)
        {
            _players.TryGetValue(id, out var value);
            return value;
        }

        public Dictionary<int, ServerPlayerObject> AllPlayers => _players;

        public void BroadcastExcept(Message msg, int excludePlayerId)
        {
            foreach (var kv in _socketToId)
            {
                if (kv.Value != excludePlayerId)
                {
                    SendToClient(kv.Key, msg);
                }
            }
        }

        public void UpdateHeartbeat(int playerId)
        {
            _lastHeartbeatTime[playerId] = Time.time;
        }
        
        private static int EffectTypeToInt(Type type)
        {  
            if (type == typeof(BurnEffect)) return 0;
            if (type == typeof(ElectrifyEffect)) return 1;
            if (type == typeof(SharpenEffect)) return 2;
            if (type == typeof(FreezeEffect)) return 3;
            return -1;
        }

    }
}