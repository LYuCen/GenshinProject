using System.Collections.Generic;

namespace IUtility
{
    public class NetworkStats
    {
        public long TotalBytes; // 总字节数
        public int PacketCount; // 总包数
        // 记录每种类型的消息分别占了多少字节
        public Dictionary<string, long> TypeBreakdown = new Dictionary<string, long>();

        public void Record(string type, int bytes)
        {
            TotalBytes += bytes;
            PacketCount++;
            if (!TypeBreakdown.ContainsKey(type)) TypeBreakdown[type] = 0;
            TypeBreakdown[type] += bytes;
        }

        public void Reset()
        {
            TotalBytes = 0;
            PacketCount = 0;
            TypeBreakdown.Clear();
        }
    }
}