using System.Collections.Generic;
using Connect.DTO;
using Connect.LobbyDTO;
using Google.FlatBuffers;

namespace Connect.Converter
{
    public static class LobbyPlayerListConverter
    {
        public static byte[] PackList(List<LobbyPlayerDto> players)
        {
            var builder = new FlatBufferBuilder(players.Count * 64 + 64);
            var offsets = new Offset<LobbyPlayerFlat>[players.Count];

            for (int i = 0; i < players.Count; i++)
            {
                var nameOff = builder.CreateString(players[i].name);
                LobbyPlayerFlat.StartLobbyPlayerFlat(builder);
                LobbyPlayerFlat.AddPlayerId(builder, players[i].id);
                LobbyPlayerFlat.AddName(builder, nameOff);
                offsets[i] = LobbyPlayerFlat.EndLobbyPlayerFlat(builder);
            }

            var vec = LobbyPlayerList.CreatePlayersVector(builder, offsets);
            LobbyPlayerList.StartLobbyPlayerList(builder);
            LobbyPlayerList.AddPlayers(builder, vec);

            var root = LobbyPlayerList.EndLobbyPlayerList(builder);
            builder.Finish(root.Value);
            
            return builder.SizedByteArray();
        }

        public static List<LobbyPlayerDto> UnpackList(byte[] buf)
        {
            var bb = new ByteBuffer(buf);
            var list = LobbyPlayerList.GetRootAsLobbyPlayerList(bb);
            var result = new List<LobbyPlayerDto>(list.PlayersLength);
            for (int i = 0; i < list.PlayersLength; i++)
            {
                var flat = list.Players(i).Value;
                result.Add(new LobbyPlayerDto
                {
                    id = flat.PlayerId,
                    name = flat.Name
                });
            }
            return result;
        }
    }
}