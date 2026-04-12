using UnityEngine;
using Connect;

namespace IUtility
{
    public class NetworkMonitor : MonoBehaviour
    {
        public static NetworkMonitor Instance;

        public NetworkStats Sent = new NetworkStats();
        public NetworkStats Received = new NetworkStats();

        // 供 UI 读取的数据
        public float CurrentFPS { get; private set; }
        public int CurrentQueueCount { get; private set; }
        public float LastSecondSentBytes { get; private set; }
        public float LastSecondReceivedBytes { get; private set; }

        private float _timer;
        private long _tempSent;
        private long _tempRecv;
        private int _frameCount;
        private float _fpsTimer;

        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        public void LogSent(string type, int bytes) => Sent.Record(type, bytes);
        public void LogReceived(string type, int bytes) => Received.Record(type, bytes);

        private void Update()
        {
            // 记录原始吞吐量（为了实时速率计算）
            _tempSent = Sent.TotalBytes; 
            _tempRecv = Received.TotalBytes;

            _frameCount++;
            _fpsTimer += Time.unscaledDeltaTime;
            CurrentQueueCount = Listener.MessageQueueCount;

            _timer += Time.unscaledDeltaTime;
            if (_timer >= 1.0f) 
            {
                Sent.UpdateTick();
                Received.UpdateTick();

                // 更新 monitor 自己的速率属性，供 UI 使用
                LastSecondSentBytes = Sent.LastSecondBytes;
                LastSecondReceivedBytes = Received.LastSecondBytes;
                
                // 计算差值得到每秒速率
                // 注意：这里我们直接用 Record 累加的值来计算
                CurrentFPS = _frameCount / _fpsTimer;
                
                // 此处简化处理：实际你可以记录 temp 变量，这里直接演示逻辑
                _frameCount = 0;
                _fpsTimer = 0;
                _timer = 0;
            }

            // 为了让速率更准，我们实时同步一下统计数据
            LastSecondSentBytes = _tempSent; // 建议在 NetworkStats 里处理速率，这里暂用原始值
            LastSecondReceivedBytes = _tempRecv;
        }

        // --- 移除了 OnGUI ---
        
        public void ResetStats()
        {
            Sent.Reset();
            Received.Reset();
        }
    }
}