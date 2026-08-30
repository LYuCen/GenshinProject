using System.Collections.Generic;
using Connect.DTO;
using Connect.ScoreboardDTO;
using Google.FlatBuffers;

namespace Connect.Converter
{
    public static class ScoreboardConverter
    { 
        public static byte[] PackInfo(ScoreboardDto dto)
        {
            var builder = new FlatBufferBuilder(2 + dto.players.Count * 16);
            var offset = new Offset<ScoreboardEntry>[dto.players.Count];

            for (int i = 0; i < dto.players.Count; i++)
            {
                var nameOff = builder.CreateString(dto.players[i].name);
                ScoreboardEntry.StartScoreboardEntry(builder);
                ScoreboardEntry.AddPlayerId(builder, dto.players[i].id);
                ScoreboardEntry.AddName(builder, nameOff);
                ScoreboardEntry.AddKill(builder, (byte)dto.players[i].kill);
                ScoreboardEntry.AddDeath(builder, (byte)dto.players[i].death);
                ScoreboardEntry.AddTeam(builder, (byte)dto.players[i].team);
                offset[i] = ScoreboardEntry.EndScoreboardEntry(builder);
            }

            var vec = Scoreboard.CreateEntriesVector(builder, offset);
            Scoreboard.StartScoreboard(builder);
            Scoreboard.AddEntries(builder, vec);
            Scoreboard.AddBlueScore(builder, (byte)dto.blueScore);
            Scoreboard.AddRedScore(builder, (byte)dto.redScore);
            Scoreboard.AddRemainTime(builder, dto.remainTime);
            Scoreboard.AddKillValue(builder, dto.killValue);
            var root = Scoreboard.EndScoreboard(builder);
            builder.Finish(root.Value);
            return builder.SizedByteArray();
        }

        public static ScoreboardDto UnPackInfo(byte[] buf)
        {
            var bb = new ByteBuffer(buf);
            var root = Scoreboard.GetRootAsScoreboard(bb);
            var vec = new List<PlayerScoreDto>(root.EntriesLength);

            for (int i = 0; i < root.EntriesLength; i++)
            {
                var e = root.Entries(i).Value;
                vec.Add(new PlayerScoreDto
                {
                    id = e.PlayerId,
                    name = e.Name,
                    kill = e.Kill,
                    death = e.Death,
                    team = e.Team
                });
            }

            var res = new ScoreboardDto
            {
                blueScore = root.BlueScore,
                redScore = root.RedScore,
                remainTime = root.RemainTime,
                killValue = root.KillValue,
                players = vec
            };
            
            return res;
        }
    }
}