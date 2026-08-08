using System;
using BaseClass;
using Connect.Core;
using Connect.Simulation;
using GameFSM.LevelFSM.PlayerFSM;
using Player.ManagerLogic;
using UnityEngine;

namespace Connect
{
    public class NetworkService : AbstractSingleton<NetworkService>
    {
        public static INetworkBridge Bridge;
        public ConnectionStateMachine StateMachine { get; private set; }
        public HeartbeatService Heartbeat { get; private set; }
        public UserClient Client { get; private set; }

        // 配置服务器信息
        public bool isSinglePlayer = true;
        public string serverIp = "127.0.0.1";
        public int serverPort = 8888;
        public PlayerManager playerManager;

        private float _reconnectTimer;
        private bool _reconnecting;


        protected override void Awake()
        {
            base.Awake();
            StateMachine = new ConnectionStateMachine();
            Heartbeat = new HeartbeatService();

            StateMachine.OnTryConnect = TryConnectToServer;
            StateMachine.OnDisconnect = DisconnectClient;
            StateMachine.OnStateChanged += OnConnectionStateChanged;

            if (isSinglePlayer)
            {
                StartSinglePlayer();
            }
            else
            {
                ConnectToServer();
            }
        }

        private void Update()
        {
            // 已连接时驱动心跳
            if (StateMachine.CurrentState == ConnectionState.Connected)
            {
                Heartbeat.Tick(Time.deltaTime);
            }
            else if (StateMachine.CurrentState == ConnectionState.Reconnecting)
            {
                _reconnectTimer += Time.deltaTime;
                if (_reconnectTimer >= StateMachine.ReconnectInterval)
                {
                    _reconnectTimer = 0;
                    Debug.Log($"[网络] 尝试重连({StateMachine.CurrentReconnectCount + 1}/{StateMachine.MaxReconnectAttempts})...");
                    StateMachine.OnTryConnect?.Invoke();
                }
            }
            
            // 监听键盘 T 键按下
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (isSinglePlayer)
                {
                    Debug.Log("正在尝试从单机模式切换到联机模式...");
                    ConnectToServer();
                }
                else
                {
                    Debug.LogWarning("当前已经是联机模式，请勿重复连接。");
                }
            }
        }

        /// <summary>
        /// 启动单机模拟器模式
        /// </summary>
        public void StartSinglePlayer()
        {
            isSinglePlayer = true;
            Bridge = new LocalServerSimulator();

            if (playerManager != null && !PlayerPool.Instance.Models.ContainsKey(Bridge.GetMyId()))
            {
                PlayerPool.Instance.RegisterLocalPlayer(Bridge.GetMyId(), playerManager);
            }
            Debug.Log("单机模式启动，ID: " + Bridge.GetMyId());
        }

        /// <summary>
        /// 连接到真实的服务器
        /// </summary>
        public void ConnectToServer()
        {
            if (StateMachine.CurrentState == ConnectionState.Connected) return;
            StateMachine.Connect();
        }

        private bool TryConnectToServer()
        {
            try
            {
                playerManager = PlayerPool.Instance.Models[1000];
                
                Client = new UserClient();
                Client.Connect(serverIp, serverPort);
                
                // 绑定 Listener 到这个新的客户端
                Listener.Instance.BindToClient(Client);
                StateMachine.OnConnectSuccess();
                PlayerPool.Instance.ClearBots();
                isSinglePlayer = false;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"连接失败：{e.Message}");
                StateMachine.OnConnectFailed();
                return false;
            }
        }

        private void DisconnectClient()
        {
            Client?.Disconnect();
            Client = null;
        }

        private void OnConnectionStateChanged(ConnectionState old, ConnectionState @new)
        {
            Debug.Log($"[网络] {old} -> {@new}");

            switch (@new)
            {
                case ConnectionState.Connected:
                    Bridge = new RealNetworkBridge(Client);
                    Debug.Log($"[Bridge] 创建 RealNetworkBridge, Client={Client?.GetHashCode()}, Client.MyId={Client?.MyId}");
                    break;
                case ConnectionState.Disconnected:
                case ConnectionState.Reconnecting:
                    Debug.Log($"[Bridge] Bridge = null, old={old}, new={@new}");
                    Bridge = null;
                    break;
            }
        }

        public void OnClientDisconnected()
        {
            StateMachine.OnConnectionLost();
        }
    }
}