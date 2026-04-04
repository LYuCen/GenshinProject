using Connect.Simulation;
using UnityEngine;

namespace Connect
{
    public class NetworkService : MonoBehaviour
    {
        public static INetworkBridge Bridge;
        public bool isSinglePlayer = true;

        private void Awake()
        {
            if (isSinglePlayer)
            {
                Bridge = new LocalServerSimulator();
                UserClient.MyId = Bridge.GetMyId();
                Debug.Log("单机模式启动，ID: " + UserClient.MyId);
            }
            else
            {
                UserClient.Connect("127.0.0.1", 8888);
                Bridge = new RealNetworkBridge();
            }
        }
    }
}