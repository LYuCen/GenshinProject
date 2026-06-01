using System;

namespace Connect.Core
{
    /// <summary>
    /// 网络连接状态机
    /// </summary>
    public class ConnectionStateMachine
    {
        public ConnectionState CurrentState { get; private set; } = ConnectionState.Disconnected;

        public event Action<ConnectionState, ConnectionState> OnStateChanged;

        // 重连配置
        public int MaxReconnectAttempts = 5;
        public float ReconnectInterval = 2f;
        public int CurrentReconnectCount { get; private set; }

        public Func<bool> OnTryConnect; // 尝试连接返回 true 表示成功
        public Action OnDisconnect;     // 执行断开逻辑
        public Action<ConnectionState> OnStateEnter; // 进入某状态

        /// <summary>
        /// 发起连接
        /// </summary>
        public void Connect()
        {
            if (CurrentState != ConnectionState.Disconnected) return;
            TransitionTo(ConnectionState.Connecting);
            OnTryConnect?.Invoke();
        }

        /// <summary>
        /// 连接成功 (由 NetworkService 在 TCP 握手完成后调用)
        /// </summary>
        public void OnConnectSuccess()
        {
            if (CurrentState == ConnectionState.Connecting || 
                CurrentState == ConnectionState.Reconnecting)
            {
                CurrentReconnectCount = 0; // 重置重连计数
            }
            
            TransitionTo(ConnectionState.Connected);
        }

        /// <summary>
        /// 连接失败 (由 NetworkService 在 TCP 失败时调用)
        /// </summary>
        public void OnConnectFailed()
        {
            if (CurrentState == ConnectionState.Connecting)
            {
                TransitionTo(ConnectionState.Disconnected);
            }
            else if (CurrentState == ConnectionState.Reconnecting)
            {
                CurrentReconnectCount++;
                // 超过最大重试次数
                if (CurrentReconnectCount >= MaxReconnectAttempts)
                {
                    TransitionTo(ConnectionState.Disconnected);
                }
            }
        }

        /// <summary>
        /// 断线检测触发 (由心跳超时或 Socket 异常触发)
        /// </summary>
        public void OnConnectionLost()
        {
            if (CurrentState != ConnectionState.Connected) return;
            
            TransitionTo(ConnectionState.Reconnecting);
        }

        /// <summary>
        /// 主动断开
        /// </summary>
        public void Disconnect()
        {
            if (CurrentState == ConnectionState.Disconnected) return;
            
            TransitionTo(ConnectionState.Disconnecting);
            OnDisconnect?.Invoke();
            CurrentReconnectCount = 0;
            TransitionTo(ConnectionState.Disconnected);
        }
        
        private void TransitionTo(ConnectionState newState)
        {
            if (CurrentState == newState) return;

            var oldState = CurrentState;
            CurrentState = newState;
            OnStateChanged?.Invoke(oldState, newState);
            OnStateEnter?.Invoke(newState);
        }
    }
}