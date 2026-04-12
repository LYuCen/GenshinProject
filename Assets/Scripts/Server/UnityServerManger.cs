using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Concurrent;
using Connect;
using Connect.FlatDTO;
using Google.FlatBuffers;

namespace Server
{
    public class UnityServerManager : MonoBehaviour
    {
        public static UnityServerManager Instance; // 单例方便全局访问
        public GameObject serverPlayerPrefab;
        
        private Socket _listener;
        private Dictionary<int, ServerPlayerObject> _players = new Dictionary<int, ServerPlayerObject>();
        private Dictionary<Socket, int> _socketToId = new Dictionary<Socket, int>();
        private FlatBufferBuilder _builder = new FlatBufferBuilder(2048);
        private int _nextId = 1000;

        // 主线程任务队列 
        private ConcurrentQueue<Action> _mainThreadTasks = new ConcurrentQueue<Action>();

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
        }

        void Update()
        {
            // 每帧在主线程处理来自 Socket 线程的任务
            while (_mainThreadTasks.TryDequeue(out var task))
            {
                task.Invoke();
            }
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
                client.NoDelay = true;         // 禁用 Nagle 算法，解决 TCP 延迟合并包的问题
                client.SendBufferSize = 65536; // 调大发送缓冲区
                client.ReceiveBufferSize = 65536; // 调大接收缓冲区
                
                int id = _nextId++;

                _mainThreadTasks.Enqueue(() => {
                    GameObject go = Instantiate(serverPlayerPrefab);
                    ServerPlayerObject spo = go.GetComponent<ServerPlayerObject>();
                    spo.Init(id, client);
                    
                    _players.Add(id, spo);
                    _socketToId.Add(client, id);
                    
                    // 发送二进制 ID
                    SendToClient(client, new Message("AllocId", BitConverter.GetBytes(id)));
                });

                _listener.BeginAccept(OnAccept, null);
            }
            catch (Exception ex) { Debug.LogError("Accept Error: " + ex.Message); }
        }

        /// <summary>
        /// 收集全场数据并广播给所有人 20tick 一次
        /// </summary>
        private void BroadcastWorldState()
        {
            if (_players.Count == 0) return;

            // 1. 准备 Builder
            _builder.Clear();
            List<Offset<PlayerDataFlat>> playerOffsets = new List<Offset<PlayerDataFlat>>();

            // 2. 收集所有玩家当前的 Offset
            foreach (var p in _players.Values)
            {
                // 调用我们之前在 ServerPlayerObject 中定义的 GetCurrentOffset
                playerOffsets.Add(p.GetCurrentOffset(_builder));
            }

            // 3. 构建 PlayerList 数组
            var vectorOffset = PlayerList.CreatePlayersVector(_builder, playerOffsets.ToArray());

            // 4. 构建 Root Table
            PlayerList.StartPlayerList(_builder);
            PlayerList.AddPlayers(_builder, vectorOffset);
            var root = PlayerList.EndPlayerList(_builder);

            // 5. 完成构建并获取字节流
            _builder.Finish(root.Value);
            byte[] finalBytes = _builder.SizedByteArray();

            // 6. 广播二进制消息
            Broadcast(new Message("AllPlayerInfo", finalBytes));
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

                // 1. 准备 Type 字符串
                byte[] typeBytes = Encoding.UTF8.GetBytes(m.type);
                // 2. 准备数据内容
                byte[] dataBytes = m.data;

                // 包体大小 = 4字节(typeLen) + Type长度 + 数据长度
                int bodySize = 4 + typeBytes.Length + dataBytes.Length;
                // 总长度 = 4字节(全包长度头) + 包体大小
                byte[] fullPacket = new byte[4 + bodySize];

                // 写入全包长度
                Buffer.BlockCopy(BitConverter.GetBytes(bodySize), 0, fullPacket, 0, 4);
                // 写入 Type 长度
                Buffer.BlockCopy(BitConverter.GetBytes(typeBytes.Length), 0, fullPacket, 4, 4);
                // 写入 Type 字符串内容
                Buffer.BlockCopy(typeBytes, 0, fullPacket, 8, typeBytes.Length);
                // 写入 FlatBuffers 数据
                Buffer.BlockCopy(dataBytes, 0, fullPacket, 8 + typeBytes.Length, dataBytes.Length);

                s.BeginSend(fullPacket, 0, fullPacket.Length, SocketFlags.None, (ar) => {
                    try { s.EndSend(ar); } catch { /* 处理掉线 */ }
                }, null);
            }
            catch { CloseClient(s); }
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
                    }
                    _socketToId.Remove(sclient);
                    
                    Debug.Log($"客户端 {playerId} 断开连接，已清理物体.");
                    byte[] exitIdBytes = BitConverter.GetBytes(playerId);
                    Broadcast(new Message("PlayerExit", exitIdBytes));
                }

                try 
                { 
                    if(sclient.Connected) sclient.Shutdown(SocketShutdown.Both); 
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
    }
}