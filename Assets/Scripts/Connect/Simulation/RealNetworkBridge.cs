namespace Connect.Simulation
{
    public class RealNetworkBridge : INetworkBridge
    {
        private readonly UserClient _client;

        public RealNetworkBridge(UserClient client)
        {
            _client = client;
        }
        
        public void SendToServer(Message msg)
        {
            _client.SendMessage(msg);
        }

        public int GetMyId() => _client.MyId;
    }
}