using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Connect.Converter;
using Connect.DTO;

namespace Connect
{
    public class LanRoomScanner
    {
        private const float ScanDuration = 2.5f;

        private UdpClient _udp;
        private readonly ConcurrentQueue<RoomInfoDto> _found = new();
        private volatile bool _running;

        public void Search()
        {
            Stop();
            _running = true;
            _udp = new UdpClient();
            _udp.EnableBroadcast = true;

            var t = new Thread(Listen) { IsBackground = true };
            t.Start();

            byte[] req = RoomInfoConverter.PackRequest();
            _udp.Send(req, req.Length, new IPEndPoint(IPAddress.Broadcast, RoomInfoConverter.UdpPort));
        }

        private void Listen()
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            DateTime deadline = DateTime.UtcNow.AddSeconds(ScanDuration);

            while (_running && DateTime.UtcNow < deadline)
            {
                try
                {
                    _udp.Client.ReceiveTimeout = 500;
                    byte[] data = _udp.Receive(ref remote);

                    var info = RoomInfoConverter.UnpackInfo(data);
                    if (info == null) continue;

                    if (string.IsNullOrEmpty(info.hostIp))
                    {
                        info.hostIp = remote.Address.ToString();
                    }

                    _found.Enqueue(info);
                }
                catch (SocketException)
                {
                    
                }
            }

            Stop();
        }

        public bool TryDequeue(out RoomInfoDto info)
        {
            return _found.TryDequeue(out info);
        }

        public void Stop()
        {
            _running = false;
            _udp?.Close();
            _udp = null;
        }
    }
}