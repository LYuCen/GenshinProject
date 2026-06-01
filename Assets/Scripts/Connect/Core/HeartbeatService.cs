using System;
using Connect.Protocol;
using UnityEngine;

namespace Connect.Core
{
    /// <summary>
    /// 客户端心跳
    /// </summary>
    public class HeartbeatService
    {
        public float SendInterval = 3f; // 3s 一次心跳
        public float TimeoutThreshold = 10f; // 10s 后无响应判定超时

        public float LastSendTime { get; private set; }
        // 最后一次收到消息的时间
        public float LastReceiveTime { get; private set; }
        // 最近一次往返延迟
        public float RTT { get; private set; }

        public Action OnTimeOut; // 心跳超时回调
        private float _currentSendTimestamp; // 本次心跳发出的客户端时间
        private bool _waitingForAck;

        public HeartbeatService()
        {
            LastReceiveTime = Time.time;
        }

        public void Tick(float deltaTime)
        {
            float now = Time.time;
            
            if (now - LastSendTime >= SendInterval)
            {
                SendHeartbeat();
            }

            // 太长时间未收到消息
            if (now - LastReceiveTime > TimeoutThreshold)
            {
                OnTimeOut?.Invoke();
            }
        }

        private void SendHeartbeat()
        {
            // 心跳包发送客户端当前时间戳，用于计算 RTT
            
            float sendTime = Time.time;
            byte[] timestampBytes = BitConverter.GetBytes(sendTime);

            if (NetworkService.Bridge == null) return;
            NetworkService.Bridge.SendToServer(new Message(MessageType.Both_Heartbeat, timestampBytes));

            LastSendTime = sendTime;
            _currentSendTimestamp = sendTime;
            _waitingForAck = true;
        }

        /// <summary>
        /// 收到心跳回包时调用
        /// </summary>
        /// <param name="data"></param>
        public void OnHeartbeatResponse(byte[] data)
        {
            if (!_waitingForAck) return;
            _waitingForAck = false;

            // 客户端发送时间
            float clientSendTime = BitConverter.ToSingle(data, 0);
            // 服务端收到心跳时间
            float serverTime = BitConverter.ToSingle(data, 4);

            RTT = Time.time - clientSendTime;
            LastReceiveTime = Time.time;
        }

        public void OnAnyMessageReceived()
        {
            LastReceiveTime = Time.time;
        }
    }
}