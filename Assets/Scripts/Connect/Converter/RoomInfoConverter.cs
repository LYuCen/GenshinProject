using Connect.DTO;
using Connect.RoomDTO;
using Google.FlatBuffers;

namespace Connect.Converter
{
    public class RoomInfoConverter
    {
        public const int UdpPort = 8889;

        public static byte[] PackInfo(RoomInfoDto dto)
        {
            var builder = new FlatBufferBuilder(256);

            var roomNameOff = builder.CreateString(dto.roomName);
            var sceneNameOff = builder.CreateString(dto.sceneName);
            var introOff = builder.CreateString(dto.roomIntroduction);
            var hostIpOff = builder.CreateString(dto.hostIp);
            
            RoomInfoFlat.StartRoomInfoFlat(builder);
            RoomInfoFlat.AddRoomName(builder, roomNameOff);
            RoomInfoFlat.AddSceneName(builder, sceneNameOff);
            RoomInfoFlat.AddRoomIntroduction(builder, introOff);
            RoomInfoFlat.AddHasPassword(builder, dto.hasPassword);
            RoomInfoFlat.AddMaxPlayers(builder, dto.maxPlayers);
            RoomInfoFlat.AddGameTime(builder, dto.gameTime);
            RoomInfoFlat.AddKillValue(builder, dto.killValue);
            RoomInfoFlat.AddCurrentPlayers(builder, dto.currentPlayers);
            RoomInfoFlat.AddHostIp(builder, hostIpOff);
            RoomInfoFlat.AddHostPort(builder, dto.hostPort);

            var infoOff = RoomInfoFlat.EndRoomInfoFlat(builder);
            
            DiscoveryPacket.StartDiscoveryPacket(builder);
            DiscoveryPacket.AddKind(builder, DiscoveryKind.Info);
            DiscoveryPacket.AddInfo(builder, infoOff);

            var root = DiscoveryPacket.EndDiscoveryPacket(builder);
            builder.Finish(root.Value);
            return builder.SizedByteArray();
        }

        public static byte[] PackRequest()
        {
            var builder = new FlatBufferBuilder(16);
            DiscoveryPacket.StartDiscoveryPacket(builder);
            DiscoveryPacket.AddKind(builder, DiscoveryKind.Request);

            var root = DiscoveryPacket.EndDiscoveryPacket(builder);
            builder.Finish(root.Value);
            return builder.SizedByteArray();
        }

        public static RoomInfoDto UnpackInfo(byte[] buf)
        {
            var bb = new ByteBuffer(buf);

            var pkt = DiscoveryPacket.GetRootAsDiscoveryPacket(bb);
            if (pkt.Kind != DiscoveryKind.Info) return null;
            if (pkt.Info == null) return null;

            var i = pkt.Info.Value;
            return new RoomInfoDto
            {
                roomName = i.RoomName,
                sceneName = i.SceneName,
                roomIntroduction = i.RoomIntroduction,
                hasPassword = i.HasPassword,
                maxPlayers = i.MaxPlayers,
                gameTime = i.GameTime,
                killValue = i.KillValue,
                currentPlayers = i.CurrentPlayers,
                hostIp = i.HostIp,
                hostPort = i.HostPort
            };
        }
    }
}