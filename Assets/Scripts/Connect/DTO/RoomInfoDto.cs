namespace Connect.DTO
{
    [System.Serializable]
    public class RoomInfoDto
    {
        public string roomName;
        public string sceneName;
        public string roomIntroduction;
        public bool hasPassword;
        public ushort maxPlayers;
        public ushort gameTime;
        public ushort killValue;
        public ushort currentPlayers;
        public string hostIp;
        public ushort hostPort;
    }
}