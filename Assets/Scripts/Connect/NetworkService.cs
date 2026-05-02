using Connect.Simulation;
using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using UnityEngine;

namespace Connect
{
    public class NetworkService : MonoBehaviour
    {
        public static INetworkBridge Bridge;
        public bool isSinglePlayer = true;
        public PlayerManager playerManager;

        // 配置服务器信息
        public string serverIp = "127.0.0.1";
        public int serverPort = 8888;

        private void Awake()
        {
            // 初始化默认模式
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
            UserClient.MyId = Bridge.GetMyId();
            Debug.Log("单机模式启动，ID: " + UserClient.MyId);
        }

        /// <summary>
        /// 连接到真实的服务器
        /// </summary>
        public void ConnectToServer()
        {
            try 
            {
                // 1. 联机前先清空旧的玩家池（销毁单机模式的假人）
                PlayerPool.Instance.ClearPool();

                UserClient.MyId = -1;
                UserClient.Connect(serverIp, serverPort, playerManager);
        
                Bridge = new RealNetworkBridge();
                isSinglePlayer = false;

                Debug.Log("正在连接服务器...");
            }
            catch (System.Exception e)
            {
                Debug.LogError("连接失败: " + e.Message);
                StartSinglePlayer();
            }
        }
    }
}