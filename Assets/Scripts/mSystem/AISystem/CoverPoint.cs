using UnityEngine;

namespace mSystem.AISystem
{
    /// <summary>
    /// 掩体信息
    /// </summary>
    public class CoverPoint : MonoBehaviour
    {
        public float score;        // 评分（离敌人距离，是否暴露等）
        public bool isOccupied;    // 是否已被占用
    }
}