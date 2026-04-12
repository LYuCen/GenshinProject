using System.Collections.Generic;

namespace IUtility
{
    public class NetworkStats
    {
        // --- 长期统计 ---
        public long TotalBytes; // 总字节数
        public int PacketCount; // 总包数
        // 记录每种类型的消息累计占了多少字节
        public Dictionary<string, long> TypeBreakdown = new Dictionary<string, long>();

        // --- 实时速率统计 (新增) ---
        public float LastSecondBytes { get; private set; }
        private long _bytesSinceLastTick;

        // 每个消息类型的实时速率 (可选，如果你想看某个协议当前的瞬间流量)
        public Dictionary<string, long> LastSecondTypeBreakdown = new Dictionary<string, long>();
        private Dictionary<string, long> _typeBytesSinceLastTick = new Dictionary<string, long>();

        public void Record(string type, int bytes)
        {
            // 1. 更新长期统计
            TotalBytes += bytes;
            PacketCount++;
            if (!TypeBreakdown.ContainsKey(type)) TypeBreakdown[type] = 0;
            TypeBreakdown[type] += bytes;

            // 2. 更新实时计数
            _bytesSinceLastTick += bytes;
            if (!_typeBytesSinceLastTick.ContainsKey(type)) _typeBytesSinceLastTick[type] = 0;
            _typeBytesSinceLastTick[type] += bytes;
        }

        /// <summary>
        /// 每秒被 NetworkMonitor 调用一次，结算这一秒的速率
        /// </summary>
        public void UpdateTick()
        {
            // 结算总速率
            LastSecondBytes = _bytesSinceLastTick;
            _bytesSinceLastTick = 0;

            // 结算细分速率 (清空旧的，存入这一秒新的)
            LastSecondTypeBreakdown.Clear();
            foreach (var kvp in _typeBytesSinceLastTick)
            {
                LastSecondTypeBreakdown[kvp.Key] = kvp.Value;
            }
            _typeBytesSinceLastTick.Clear();
        }

        public void Reset()
        {
            TotalBytes = 0;
            PacketCount = 0;
            TypeBreakdown.Clear();
            
            LastSecondBytes = 0;
            _bytesSinceLastTick = 0;
            LastSecondTypeBreakdown.Clear();
            _typeBytesSinceLastTick.Clear();
        }
    }
}