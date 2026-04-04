using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Connect.DTO;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Concurrent;
using Connect;

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
                int id = _nextId++;

                // 将创建物体的逻辑丢进主线程队列
                _mainThreadTasks.Enqueue(() => {
                    // 实例化角色并将其初始化
                    GameObject go = Instantiate(serverPlayerPrefab);
                    ServerPlayerObject spo = go.GetComponent<ServerPlayerObject>();
                    spo.Init(id, client);
                    
                    _players.Add(id, spo);
                    _socketToId.Add(client, id);
                    
                    SendToClient(client, new Message("AllocId", id.ToString()));
                    Debug.Log($"玩家 {id} 已连接并生成物理实体");
                });

                // 继续异步监听下一个玩家连接
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

            List<PlayerDataDto> dtos = new List<PlayerDataDto>();
            foreach (var p in _players.Values)
            {
                dtos.Add(p.GetCurrentDto());
            }

            string json = JsonConvert.SerializeObject(dtos);
            Message msg = new Message("AllPlayerInfo", json);

            // 广播给所有人
            Broadcast(msg);
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
                string str = JsonConvert.SerializeObject(m);
                byte[] bytes = Encoding.UTF8.GetBytes(str + '&');
                s.Send(bytes);
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
                    Broadcast(new Message("PlayerExit", playerId.ToString()));
                }

                try 
                { 
                    if(sclient.Connected) sclient.Shutdown(SocketShutdown.Both); 
                    sclient.Close(); 
                } catch { }
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