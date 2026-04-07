using UnityEngine;

namespace IUtility
{
    public class NetworkMonitor : MonoBehaviour
    {
        // 单例引用
        public static NetworkMonitor Instance;

        // 发送和接收的统计实例
        public NetworkStats Sent = new NetworkStats();
        public NetworkStats Received = new NetworkStats();

        // 实时速率变量
        public float LastSecondSentBytes { get; private set; }
        public float LastSecondReceivedBytes { get; private set; }

        private float _timer;
        private long _tempSent;
        private long _tempRecv;

        private void Awake()
        {
            // 经典的单机实现，确保全局只有一个统计中心
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // 当 UserClient 发送消息时调用此方法
        public void LogSent(string type, int bytes)
        {
            Sent.Record(type, bytes);
            _tempSent += bytes;
        }

        // 当 Listener 收到消息时调用此方法
        public void LogReceived(string type, int bytes)
        {
            Received.Record(type, bytes);
            _tempRecv += bytes;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= 1.0f) // 每秒结算一次速率
            {
                LastSecondSentBytes = _tempSent;
                LastSecondReceivedBytes = _tempRecv;
                _tempSent = 0;
                _tempRecv = 0;
                _timer = 0;
            }
        }

        public void ResetStats()
        {
            Sent.Reset();
            Received.Reset();
        }
    }
}