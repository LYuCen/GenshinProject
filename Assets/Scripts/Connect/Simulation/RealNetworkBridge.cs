namespace Connect.Simulation
{
    public class RealNetworkBridge : INetworkBridge
    {
        public void SendToServer(Message msg)
        {
            UserClient.SendMessage(msg);
        }

        public int GetMyId() => UserClient.MyId;
    }
}