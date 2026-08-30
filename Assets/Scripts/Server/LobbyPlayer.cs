using System.Net.Sockets;
using Server.Enum;

namespace Server
{
    public class LobbyPlayer
    {
        public int Id;
        public string Name;
        public Socket Socket;
        public bool IsHost;
        public PlayerState State;
        public int Team;
        public int Kill;
        public int Death;
        public int LastAttackerId;
    }
}