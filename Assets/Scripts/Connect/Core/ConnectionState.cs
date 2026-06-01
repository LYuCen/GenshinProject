namespace Connect.Core
{
    public enum ConnectionState
    {
        Disconnected,   // 未连接
        Connecting,     // 正在连接中
        Connected,      // 已连接
        Disconnecting,  // 正在连接
        Reconnecting    // 断线后尝试重连
    }
}