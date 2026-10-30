using System;
using System.Net.Sockets;
using System.Threading;
using Connect.Core;
using Connect.Protocol;
using Cysharp.Threading.Tasks;
using UnityEngine;
using IUtility;

namespace Connect
{
    /// <summary>
    /// 玩家客户端
    /// </summary>
    public class UserClient
    {
        private Socket _socket;
        public int MyId = -1;

        public event Action<Message> OnMessageReceived;
        public event Action OnDisconnected;
        
        public void Connect(string ip, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ip, port);
            
            _socket.NoDelay = true; // 禁用 Nagle 算法，解决 TCP 延迟合并包的问题
            _socket.SendBufferSize = 65536; // 调大发送缓冲区
            _socket.ReceiveBufferSize = 65536; // 调大接收缓冲区
            
            StartListen();
        }

        public async UniTask ConnectAsync(string ip, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await UniTask.RunOnThreadPool(() => _socket.Connect(ip, port));

            _socket.NoDelay = true; // 禁用 Nagle 算法
            _socket.SendBufferSize = 65536; // 发送缓冲区
            _socket.ReceiveBufferSize = 65536; // 接收缓冲区
            StartListen();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_socket != null && _socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                _socket = null;
                MyId = -1;
                OnDisconnected?.Invoke();
            }
        }

        public void SendMessage(Message msg)
        {
            if (_socket == null || !_socket.Connected)
            {
                Debug.LogError("未连接到服务器");
                return;
            }

            try 
            {
                // 获取 FlatBuffers 的二进制数据
                byte[] dataBytes = msg.data ?? new byte[0]; // 防止空引用

                // --- 核心：构造二进制包 ---
                // 包体大小 = 1字节(type) + FlatBuffers数据长度
                int bodySize = 2 + dataBytes.Length;
                
                // 总发送缓冲区 = 4字节(总包长头) + 包体大小
                byte[] fullPacket = new byte[4 + bodySize];

                // [0-3字节]: 写入全包长度 (BitConverter 默认小端序)
                Buffer.BlockCopy(BitConverter.GetBytes(bodySize), 0, fullPacket, 0, 4);
                
                // [4-5]: 消息类型枚举
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)msg.type), 0, fullPacket, 4, 2);
                
                // [6...字节]: FlatBuffers数据
                Buffer.BlockCopy(dataBytes, 0, fullPacket, 6, dataBytes.Length);
                
                _socket.Send(fullPacket);

                // 监听网络流量统计
                if (NetworkMonitor.Instance)
                {
                    NetworkMonitor.Instance.LogSent(msg.type.ToString(), fullPacket.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("发送消息失败: " + ex.Message);
            }
        }

        private void StartListen()
        {
            Thread t = new Thread(ListenLoop);
            t.IsBackground = true;
            t.Start();
        }

        private void ListenLoop()
        {
            byte[] headerBuff = new byte[4];
            while (_socket != null && _socket.Connected)
            {
                try
                {
                    int headLen = _socket.Receive(headerBuff, 0, 4, SocketFlags.None);
                    if (headLen < 4) break;

                    int bodySize = BitConverter.ToInt32(headerBuff, 0);
                    byte[] bodyBuff = new byte[bodySize];
                    int received = 0;
                    while (received < bodySize)
                    {
                        int count = _socket.Receive(bodyBuff, received, bodySize - received, SocketFlags.None);
                        if (count <= 0) break;
                        received += count;
                    }

                    Message msg = UnpackMessage(bodyBuff);
                    if (msg.type == MessageType.S2C_Pong) // Pong接收后就地计算延迟
                    {
                        PingService.Instance?.OnPong(msg.data, System.Diagnostics.Stopwatch.GetTimestamp());
                    }
                    else OnMessageReceived?.Invoke(msg);
                }
                catch
                {
                    break;
                }
            }
            OnDisconnected?.Invoke();
        }
        
        private Message UnpackMessage(byte[] bodyBuff)
        {
            MessageType type = (MessageType)BitConverter.ToUInt16(bodyBuff, 0);      
            int dataSize = bodyBuff.Length - 2;       
            byte[] data = new byte[dataSize];
            Buffer.BlockCopy(bodyBuff, 2, data, 0,    
                dataSize);
            return new Message(type, data);
        }

    }
}