namespace Connect
{
    public interface INetworkBridge
    {
        void SendToServer(Message msg);
        int GetMyId();
    }
}