using System;
using System.Diagnostics;
using Connect.Protocol;

namespace Connect.Core
{
    /// <summary>
    /// 4HZ 检测服务器延迟
    /// </summary>
    public class PingService
    {
        public static PingService Instance { get; private set; }

        private const float PingInterval = 0.25f; // 4HZ

        private int _sequece;
        private long _lastPingTicks;
        private bool _hasSent;

        private volatile float _rtt;
        public float RTT => _rtt;
        public float OneWayLatency => _rtt * 0.5f;

        public PingService()
        {
            Instance = this;
        }

        public void Tick()
        {
            long now = Stopwatch.GetTimestamp();
            double sinceLast = (now - _lastPingTicks) / (double)Stopwatch.Frequency;
            if (!_hasSent || sinceLast >= PingInterval) SendPing(now);
        }

        private void SendPing(long nowTicks)
        {
            _sequece++;

            byte[] payload = new byte[12];
            BitConverter.GetBytes(_sequece).CopyTo(payload, 0);
            BitConverter.GetBytes(nowTicks).CopyTo(payload, 4);

            if (NetworkService.Bridge == null) return;

            NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_Ping, payload));

            _lastPingTicks = nowTicks;
            _hasSent = true;
        }

        public void OnPong(byte[] data, long recvTicks)
        {
            long sendTicks = BitConverter.ToInt64(data, 4);
            _rtt = (float)((recvTicks - sendTicks) / (double)Stopwatch.Frequency);
        }
    }
}