using Connect.Protocol;

namespace Connect
{
    [System.Serializable]
    public class Message
    {
        public MessageType type;
        public byte[] data;

        public Message(MessageType type, byte[] data) {
            this.type = type;
            this.data = data ?? new byte[0];
        }
    }
}