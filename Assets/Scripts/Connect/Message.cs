namespace Connect
{
    [System.Serializable]
    public class Message
    {
        public string type;
        public byte[] data;

        public Message(string type, byte[] data) {
            this.type = type;
            this.data = data;
        }
    }
}