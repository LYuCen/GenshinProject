using System.Collections.Generic;

namespace Connect.DTO
{
    [System.Serializable]
    public class ScoreboardDto
    {
        public int blueScore;
        public int redScore;
        public List<PlayerScoreDto> players;
        public ushort remainTime;
        public ushort killValue;
    }
}