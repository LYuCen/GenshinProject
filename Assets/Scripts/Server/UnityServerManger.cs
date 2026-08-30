using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using AI;
using AI.VisualRequest.VisualRequest;
using AssetBundleFramework.Core.Resource;
using Connect;
using Connect.Converter;
using Connect.DTO;
using Connect.Protocol;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.ServerTimeSystem;
using Server.Enum;
using Debug = UnityEngine.Debug;

namespace Server
{
    public class UnityServerManager : MonoBehaviour
    {
        public static UnityServerManager Instance; // 单例方便全局访问
        public GameObject serverPlayerPrefab;
        public uint ServerTickCount { get; private set; }

        private Socket _listener;
        private Dictionary<int, ServerPlayerObject> _players = new Dictionary<int, ServerPlayerObject>();
        private Dictionary<Socket, int> _socketToId = new Dictionary<Socket, int>();
        private int _nextId = 1000;

        // 主线程任务队列 
        private ConcurrentQueue<Action> _mainThreadTasks = new ConcurrentQueue<Action>();

        private Dictionary<int, float> _lastHeartbeatTime = new Dictionary<int, float>();
        private const float HeartbeatTimeout = 10f;
        
        private struct ElementSyncState
        {
            public bool Enable;
            public float AttachTime; // 攻击时刻
        }
        private Dictionary<int, ElementSyncState[]> _prevElementState = new();
        
        private Stopwatch _serverClock = new Stopwatch();
        private long _lastTickTime;
        private long _fixLastTickTime;
        private const double TickRate = 0.05; //20Hz
        private const double TickFix = 0.02f; // 50Hz
        private const int MaxTaskPerFrame = 5; // 最多一次性处理的网络连接请求数
        private float ServerTime => _serverClock.ElapsedMilliseconds / 1000f;

        private ServerState _state = ServerState.Lobby;
        private Dictionary<int, LobbyPlayer> _lobbyPlayers = new();
        private bool _startingGame;

        private RoomInfoDto _roomInfo = new RoomInfoDto
        {
            gameTime = 5,
            killValue = 5,
        };
        private UdpClient _udpDiscovery;
        private byte[] _roomInfoBytes;
        private readonly object _roomInfoLock = new();
        
        // 队伍信息
        private readonly List<LobbyPlayer> _blueTeam = new();
        private readonly List<LobbyPlayer> _redTeam = new();
        private int _blueScore;
        private int _redScore;
        private float _gameEndTime;

        [SerializeField] private Transform blueRebirthPoint;
        [SerializeField] private Transform redRebirthPoint;
        [SerializeField] private float spawnSafetyRadius = 1f;

        private List<PointVisualRequest> _bluePointVisualizers = new();
        private List<PointVisualRequest> _redPointVisualizers = new();
        private Queue<Vector3> _blueSpawnQueue = new();
        private Queue<Vector3> _redSpawnQueue = new();

        [SerializeField] private bool editorAutoStart = true;
        private bool _autoStartOnJoin;
        

        void Awake()
        {
            Instance = this;
            ResourceManager.EnsureInitialized();
            TimeSystem.Instance.autoTick = false;

            // 手动开服时自带 -autoStart
            _autoStartOnJoin = Array.IndexOf(Environment.GetCommandLineArgs(), "-autoStart") >= 0 ||
                               (Application.isEditor && editorAutoStart);
        }

        void Start()
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, 8888));
            _listener.Listen(10);
            Debug.Log("<color=green>Unity Dedicated Server Started on port 8888...</color>");
            
            _serverClock.Start();
            _lastTickTime = _serverClock.ElapsedTicks;
            _fixLastTickTime = _serverClock.ElapsedTicks;

            // 异步接收玩家连接
            _listener.BeginAccept(OnAccept, null);
            StartUdpDiscovery();

            for (int i = 0; i < blueRebirthPoint.childCount; i++)
            {
                var point = new PointVisualRequest
                {
                    PointColor = Color.blue,
                    PointPos = blueRebirthPoint.GetChild(i).position,
                    Radius = 0.1f,
                    Text = "蓝队复活点"
                };
                _bluePointVisualizers.Add(point);
                VisualizerManager.Instance.Register(point);
            }
            
            for (int i = 0; i < redRebirthPoint.childCount; i++)
            {
                var point = new PointVisualRequest
                {
                    PointColor = Color.red,
                    PointPos = redRebirthPoint.GetChild(i).position,
                    Radius = 0.1f,
                    Text = "红队复活点"
                };
                _redPointVisualizers.Add(point);
                VisualizerManager.Instance.Register(point);
            }
        }

        private void InitSpawnQueues()
        {
            _blueSpawnQueue.Clear();
            _redSpawnQueue.Clear();

            if (blueRebirthPoint != null)
            {
                for (int i = 0; i < blueRebirthPoint.childCount; i++)
                {
                    _blueSpawnQueue.Enqueue(blueRebirthPoint.GetChild(i).position);
                }
            }

            if (redRebirthPoint != null)
            {
                for (int i = 0; i < redRebirthPoint.childCount; i++)
                {
                    _redSpawnQueue.Enqueue(redRebirthPoint.GetChild(i).position);
                }
            }
        }

        void Update()
        {
            ProcessMainThreadTasks();
            
            long now = _serverClock.ElapsedTicks;
            double elapsed = (double)(now - _lastTickTime) / Stopwatch.Frequency;
            while(elapsed >= TickRate)
            {
                _lastTickTime += (long)(TickRate * Stopwatch.Frequency);
                ServerTick();
                elapsed = (double)(_serverClock.ElapsedTicks - _lastTickTime) / Stopwatch.Frequency;
            }

            double fixElapsed = (double)(now - _fixLastTickTime) / Stopwatch.Frequency;
            while (fixElapsed >= TickFix)
            {
                _fixLastTickTime += (long)(TickFix * Stopwatch.Frequency);
                ServerFixTick();
                fixElapsed = (double)(_serverClock.ElapsedTicks - _fixLastTickTime) / Stopwatch.Frequency;
            }
            
            
            for (int i = 0; i < blueRebirthPoint.childCount; i++)
            {
                _bluePointVisualizers[i].PointPos = blueRebirthPoint.GetChild(i).position;
            }
            
            for (int i = 0; i < redRebirthPoint.childCount; i++)
            {
                _redPointVisualizers[i].PointPos = redRebirthPoint.GetChild(i).position;
            }
        }

        private void ServerTick()
        {
            CheckHeartbeatTimeout();
            CheckGameEnd();
            BroadcastWorldState();
            TickElementBuffs();
        }

        private void ServerFixTick()
        {
            ServerTickCount++;
            TimeSystem.Instance.Tick(ServerTickCount * 0.02f);
            foreach (var kv in _players)
            {
                kv.Value.ServerTick(0.02f);
            }
        }

        private void ProcessMainThreadTasks()
        {
            int count = 0;

            while(count < MaxTaskPerFrame && _mainThreadTasks.TryDequeue(out var task))
            {
                try
                {
                    task.Invoke();
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }

                count++;
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
                client.NoDelay = true; // 禁用 Nagle 算法，解决 TCP 延迟合并包的问题
                client.SendBufferSize = 65536;
                client.ReceiveBufferSize = 65536;
                
                _mainThreadTasks.Enqueue(() =>
                {
                    int id = _nextId++;
                    _lastHeartbeatTime[id] = ServerTime;
                    
                    var lp = new LobbyPlayer
                    {
                        Id = id,
                        Name = $"玩家{id}",
                        Socket = client,
                        IsHost = _lobbyPlayers.Count == 0
                    };
                    
                    lp.State = _state == ServerState.Playing ? PlayerState.LoadingScene : PlayerState.InLobby;
                    AddToTeam(lp);
                    _lobbyPlayers[id] = lp;
                    RefreshRoomInfoBytes();
                    _socketToId.Add(client, id);
                    BroadcastLobbyPlayers();
                    StartReceiveLoop(client, id);
                    
                    SendToClient(client, new Message(MessageType.S2C_AllocId, BitConverter.GetBytes(id)));
                    byte[] roomInfoBytes;
                    lock (_roomInfoLock)
                    {
                        roomInfoBytes = _roomInfoBytes;
                    }

                    if (roomInfoBytes != null)
                        SendToClient(client, new Message(MessageType.S2C_RoomInfo, roomInfoBytes));
                    
                    // 中途加入
                    if (lp.State == PlayerState.LoadingScene)
                    {
                        SendToClient(client, new Message(MessageType.S2C_StartGame, Array.Empty<byte>()));
                    }
                    
                    Debug.Log($"玩家{id}加入{(lp.State == PlayerState.LoadingScene ? "游戏(中途)" : "大厅")}");
                    if (_autoStartOnJoin && _state == ServerState.Lobby && lp.IsHost)
                    {
                        HandleStartGame(id);
                    }
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
                        Broadcast(ElementBuffConverter.PackAttach(kv.Key, i, e.Level, (int)e.GetVal(ctrl.TimeProvider()), e.Capacity));
                    }
                    else if (prevState[i].Enable && nowEnabled && !Mathf.Approximately(prevState[i].AttachTime, nowAttachTime))
                    {
                        Broadcast(ElementBuffConverter.PackAttach(kv.Key, i, e.Level, (int)e.GetVal(ctrl.TimeProvider()), e.Capacity));
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
                    byte[] exitIdBytes = BitConverter.GetBytes(playerId);

                    bool wasHost = false;
                    if (_lobbyPlayers.TryGetValue(playerId, out var val))
                    {
                        wasHost = val.IsHost;
                        (val.Team == 0 ? _blueTeam : _redTeam).Remove(val); // 玩家移除队伍
                        if (val.State == PlayerState.InLobby)
                        {
                            BroadcastLobby(new Message(MessageType.S2C_PlayerExit, exitIdBytes));
                        }
                        else
                        {
                            Broadcast(new Message(MessageType.S2C_PlayerExit, exitIdBytes));
                        }
                    }
                    
                    _lobbyPlayers.Remove(playerId);
                    RefreshRoomInfoBytes();
                    BroadcastLobbyPlayers();
                    _lastHeartbeatTime.Remove(playerId);
                    Debug.Log($"客户端 {playerId} 断开连接");

                    if (wasHost)
                    {
                        Debug.Log("房主已退出，服务器关闭");
                        Application.Quit();
                    }
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
        /// 将消息发给在大厅的所有玩家
        /// </summary>
        /// <param name="msg"></param>
        public void BroadcastLobby(Message msg)
        {
            var targets = new Dictionary<Socket, int>(_socketToId);
            foreach (var kv in targets)
            {
                if (_lobbyPlayers.TryGetValue(kv.Value, out LobbyPlayer value) && value.State == PlayerState.InLobby)
                {
                    SendToClient(kv.Key, msg);
                }
            }
        }

        public void ConfigureRoom(string roomName, string sceneName, string introduction, bool hasPassword,
            int maxPlayers, int gameTime, int killValue)
        {
            _roomInfo = new RoomInfoDto
            {
                roomName = roomName,
                sceneName = sceneName,
                roomIntroduction = introduction,
                hasPassword = hasPassword,
                maxPlayers = (ushort)maxPlayers,
                gameTime = (ushort)gameTime,
                killValue = (ushort)killValue,
                hostIp = GetLocalIP(),
                hostPort = 8888
            };
            RefreshRoomInfoBytes();
        }

        private void RefreshRoomInfoBytes()
        {
            if (_roomInfo == null) return;
            _roomInfo.currentPlayers = (ushort)_lobbyPlayers.Count;
            lock (_roomInfoLock)
            {
                _roomInfoBytes = RoomInfoConverter.PackInfo(new RoomInfoDto
                {
                    roomName = _roomInfo.roomName,
                    sceneName = _roomInfo.sceneName,
                    roomIntroduction = _roomInfo.roomIntroduction,
                    hasPassword = _roomInfo.hasPassword,
                    maxPlayers = _roomInfo.maxPlayers,
                    gameTime = _roomInfo.gameTime,
                    killValue = _roomInfo.killValue,
                    currentPlayers = _roomInfo.currentPlayers,
                    hostIp = _roomInfo.hostIp,
                    hostPort = _roomInfo.hostPort
                });
            }
        }

        private void StartUdpDiscovery()
        {
            try
            {
                _udpDiscovery = new UdpClient(new IPEndPoint(IPAddress.Any, RoomInfoConverter.UdpPort));
                var t = new Thread(DiscoveryLoop) { IsBackground = true };
                t.Start();
            }
            catch (Exception e)
            {
                Debug.LogWarning("UDP 房间发现服务启动失败: " + e.Message);
            }
        }

        private void DiscoveryLoop()
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] req = _udpDiscovery.Receive(ref remote);
                    byte[] resp;
                    lock (_roomInfoLock)
                    {
                        resp = _roomInfoBytes;
                    }

                    if (resp == null) continue;
                    _udpDiscovery.Send(resp, resp.Length, remote);
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }
        
        private static string GetLocalIP()
        {
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) return ip.ToString();
            }
            return "127.0.0.1";
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
            float now = ServerTime;
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

                if (_lobbyPlayers.TryGetValue(id, out var lp))
                {
                    CloseClient(lp.Socket);
                }
            }
        }

        /// <summary>
        /// 实例服务端模拟的玩家
        /// </summary>
        private void SpawnServerPlayer(int id, Socket client)
        {
            GameObject go = Instantiate(serverPlayerPrefab);
            ServerPlayerObject spo = go.GetComponent<ServerPlayerObject>();
            spo.Init(id, client);

            // 初始化复活点
            Vector3 spawnPos = GetRebirthPosition(id);
            var cc = go.GetComponent<CharacterController>();   
            if (cc != null) cc.enabled = false;
            go.transform.position = spawnPos;
            if (cc != null) cc.enabled = true;
            spo.State.position = spawnPos;

            _players.Add(id, spo);

            var ctrl = spo.GetComponent<ElementBuffController>();      
            ctrl.OnEffectAdded += (type) => Broadcast(ElementBuffConverter.PackStatusEffect(id, EffectTypeToInt(type), true, 0));   
            ctrl.OnEffectRemoved += (type) => Broadcast(ElementBuffConverter.PackStatusEffect(id, EffectTypeToInt(type), false, 0));  
        }

        private void StartReceiveLoop(Socket client, int id)
        {
            var t = new Thread(() => ReceiveLoop(client, id));
            t.IsBackground = true;
            t.Start();
        }

        private void ReceiveLoop(Socket client, int id)
        {
            while (client != null && client.Connected)
            {
                try
                {
                    byte[] head = new byte[4];
                    if (!ReadExact(client, head, 4)) break;

                    int size = BitConverter.ToInt32(head, 0);
                    byte[] body = new byte[size];
                    if (!ReadExact(client, body, size)) break;

                    MessageType type = (MessageType)BitConverter.ToUInt16(body, 0);
                    byte[] data = new byte[size - 2];
                    Buffer.BlockCopy(body, 2, data, 0, data.Length);
                    EnqueueTask(() => HandleMessage(id, client, type, data));
                }
                catch(Exception ex)
                {
                    Debug.Log($"玩家 {id} 接收异常： {ex.Message}");
                    break;
                }
            }
            EnqueueTask(() => CloseClient(client));
        }

        private bool ReadExact(Socket s, byte[] buf, int size)
        {
            int recived = 0;
            while (recived < size)
            {
                int n = s.Receive(buf, recived, size - recived, SocketFlags.None);
                if (n <= 0) return false;
                recived += n;
            }

            return true;
        }

        private void HandleMessage(int id, Socket socket, MessageType type, byte[] data)
        {
            switch (type)
            {
                case MessageType.Both_Heartbeat:
                    UpdateHeartbeat(id);
                    byte[] resp = new byte[8];
                    Buffer.BlockCopy(data, 0, resp, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(ServerTickCount), 0, resp, 4, 4);
                    SendToClient(socket, new Message(MessageType.Both_Heartbeat, resp));
                    break;
                
                case MessageType.C2S_StartGame:
                    Debug.Log($"[Server] 收到 C2S_StartGame, id={id}"); 
                    HandleStartGame(id);
                    break;
                
                case MessageType.C2S_SceneLoaded:
                    HandleSceneLoaded(id, socket);
                    break;
                
                case MessageType.C2S_CreateRoom:
                    var dto = RoomInfoConverter.UnpackInfo(data);
                    if (dto != null)
                    {
                        ConfigureRoom(dto.roomName, dto.sceneName, dto.roomIntroduction,
                            dto.hasPassword, dto.maxPlayers, dto.gameTime, dto.killValue);
                        if (_lobbyPlayers.TryGetValue(id, out var creator))
                        {
                            // 创建房间的玩家设为房主
                            creator.IsHost = true;
                        }

                    }
                    break;
                
                case MessageType.C2S_GetLobbyPlayers:
                    HandleGetLobbyPlayers(id, socket);
                    break;
                
                case MessageType.C2S_GetRoomInfo:
                    byte[] roomBytes;
                    lock (_roomInfoLock)
                    {
                        roomBytes = _roomInfoBytes;
                    }

                    if (roomBytes != null)
                    {
                        SendToClient(socket, new Message(MessageType.S2C_RoomInfo, roomBytes));
                    }
                    break;
                
                default:
                    if (_players.TryGetValue(id, out var spo))
                    {
                        spo.HandleBinaryMessage(type, data);
                    }
                    break;
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void HandleStartGame(int id)
        {
            Debug.Log(
                $"[Server] HandleStartGame id={id} state={_state} inLobby={_lobbyPlayers.ContainsKey(id)} isHost={(_lobbyPlayers.TryGetValue(id, out var t) ? t.IsHost : false)}");
            if (_state != ServerState.Lobby) { Debug.Log("[Server] 拦截① state != Lobby"); return; }
            if (!_lobbyPlayers.TryGetValue(id, out var lp) || !lp.IsHost) { Debug.Log("[Server] 拦截② 非房主或不在列表"); return; }
            
            _state = ServerState.Playing;
            _startingGame = true;
            _blueScore = 0;
            _redScore = 0;
            _gameEndTime = ServerTime + _roomInfo.gameTime * 60;
            InitSpawnQueues();

            foreach (var p in _lobbyPlayers.Values)
            {
                p.Kill = 0;
                p.Death = 0;
                p.LastAttackerId = -1; 
                p.State = PlayerState.LoadingScene;
                SendToClient(p.Socket, new Message(MessageType.S2C_StartGame, Array.Empty<byte>()));
            }
        }

        private void HandleSceneLoaded(int id, Socket socket)
        {
            if (!_lobbyPlayers.TryGetValue(id, out var lp)) return;
            if (lp.State != PlayerState.LoadingScene) return;

            lp.State = PlayerState.InGame;
            SpawnServerPlayer(id, socket);

            if (_startingGame)
            {
                // 开始游戏，等待所有玩家加载完毕
                foreach (var p in _lobbyPlayers.Values)
                {
                    if (p.State == PlayerState.LoadingScene) return;
                }
                _startingGame = false;
                Broadcast(new Message(MessageType.S2C_AllPlayerInfo, PlayerDataConverter.PackAllPlayers(_players)));
                Broadcast(new Message(MessageType.S2C_Scoreboard, ScoreboardConverter.PackInfo(BuildScoreboard()))); 
            }
            else
            {
                // 中途加入
                SendToClient(socket, new Message(MessageType.S2C_AllPlayerInfo, PlayerDataConverter.PackAllPlayers(_players)));
                Broadcast(new Message(MessageType.S2C_Scoreboard, ScoreboardConverter.PackInfo(BuildScoreboard()))); 
                var spo = GetPlayer(id);
                BroadcastExcept(
                    new Message(MessageType.S2C_PlayerSpawn, PlayerDataConverter.PackSinglePlayer(spo.State)), id);
                
            }
        }

        /// <summary>
        /// 通知客户端更新大厅玩家数据
        /// </summary>
        private void HandleGetLobbyPlayers(int id, Socket socket)
        {
            SendToClient(socket,
                new Message(MessageType.S2C_LobbyPlayers, LobbyPlayerListConverter.PackList(BuildLobbyPlayerList())));
        }

        private List<LobbyPlayerDto> BuildLobbyPlayerList()
        {
            var list = new List<LobbyPlayerDto>();
            foreach (var kv in _lobbyPlayers)
            {
                list.Add(new LobbyPlayerDto
                {
                    id = kv.Key,
                    name = kv.Value.Name
                });
            }

            return list;
        }
        
        /// <summary>
        /// 通知大厅所有玩家更新玩家列表
        /// </summary>
        private void BroadcastLobbyPlayers()
        {
            BroadcastLobby(new
                Message(MessageType.S2C_LobbyPlayers, LobbyPlayerListConverter.PackList(BuildLobbyPlayerList())));
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
            _lastHeartbeatTime[playerId] = ServerTime;
        }

        public int GetTeam(int playerId)
        {
            return _lobbyPlayers.TryGetValue(playerId, out var lp) ? lp.Team : -1;
        }

        private void AddToTeam(LobbyPlayer lp)
        {
            lp.Team = _blueTeam.Count <= _redTeam.Count ? 0 : 1;
            if(lp.Team == 0) _blueTeam.Add(lp);
            else _redTeam.Add(lp);
        }

        /// <summary>
        /// 最后一个攻击者
        /// </summary>
        public void SetLastAttacker(int victimId, int attackerId)
        {
            if (_lobbyPlayers.TryGetValue(victimId, out var v))
                v.LastAttackerId = attackerId;
        }

        /// <summary>
        /// 对局中死亡，death+1， 敌方队伍分+1
        /// </summary>
        /// <param name="victimId"></param>
        public void OnPlayerDeath(int victimId)
        {
            if (!_lobbyPlayers.TryGetValue(victimId, out var victim)) return;

            victim.Death++;

            int killerId = victim.LastAttackerId;
            if (killerId >= 0 && killerId != victimId && _lobbyPlayers.TryGetValue(killerId, out var killer) &&
                killer.Team != victim.Team)
            {
                killer.Kill++;
            }

            if (victim.Team == 0) _redScore++;
            else _blueScore++;

            Broadcast(new Message(MessageType.S2C_Scoreboard, ScoreboardConverter.PackInfo(BuildScoreboard())));
        }

        /// <summary>
        /// 获取玩家复活位置
        /// </summary>
        public Vector3 GetRebirthPosition(int playerId)
        {
            int team = GetTeam(playerId);
            var queue = team == 1 ? _redSpawnQueue : _blueSpawnQueue;
            if(queue.Count == 0) return Vector3.zero;

            Vector3 fallback = queue.Peek();
            
            for (int i = 0; i < queue.Count; i++)
            {
                Vector3 pos = queue.Dequeue();
                queue.Enqueue(pos);
                if (IsSpawnPointSafe(pos, playerId))return pos;
            }
            
            return fallback;
        }

        private bool IsSpawnPointSafe(Vector3 pos, int respawnPlayerId)
        {
            float sqrRadius = spawnSafetyRadius * spawnSafetyRadius;
            foreach (var kv in _players)
            {
                var p = kv.Value;
                if (p.playerId == respawnPlayerId) continue; // 跳过复活者自己
                if (p.isDead) continue; // 死人不算占点

                if ((p.transform.position - pos).sqrMagnitude < sqrRadius) return false;
            }

            return true;
        }

        private ScoreboardDto BuildScoreboard()
        {
            var dto = new ScoreboardDto
            {
                blueScore = _blueScore,
                redScore = _redScore,
                players = new List<PlayerScoreDto>(_lobbyPlayers.Count),
                killValue = _roomInfo?.killValue ?? 0,
                remainTime = (ushort)(_gameEndTime - ServerTime)
            };
            foreach (var kv in _lobbyPlayers)
            {
                dto.players.Add(new PlayerScoreDto
                {
                    id = kv.Key,
                    name = kv.Value.Name,
                    kill = kv.Value.Kill,
                    death = kv.Value.Death,
                    team = kv.Value.Team
                });
            }

            return dto;
        }

        private void CheckGameEnd()
        {
            if (_state != ServerState.Playing) return;
            if (_blueScore >= _roomInfo.killValue || _redScore >= _roomInfo.killValue || ServerTime >= _gameEndTime)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            Debug.Log("对局结束，所有玩家返回大厅");
            _state = ServerState.Lobby;
            _startingGame = false;

            // 销毁服务端模拟的玩家
            foreach (var kv in _players)
            {
                if(kv.Value) Destroy(kv.Value.gameObject);
            }
            _players.Clear();
            _prevElementState.Clear();

            int hostId = -1;
            foreach (var lp in _lobbyPlayers.Values)
            {
                if (lp.IsHost) hostId = lp.Id;
                lp.State = PlayerState.InLobby;
                lp.Kill = 0;
                lp.Death = 0;
                lp.LastAttackerId = -1;
            }

            _blueScore = 0;
            _redScore = 0;
            
            RefreshRoomInfoBytes();
            
            Broadcast(new Message(MessageType.S2C_GameOver, BitConverter.GetBytes(hostId)));
            BroadcastLobbyPlayers();

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