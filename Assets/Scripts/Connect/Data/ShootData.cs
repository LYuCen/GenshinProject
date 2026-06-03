using UnityEngine;

namespace Connect.Data
{
    [System.Serializable]
    public class ShootData
    {
        public int shooterId;       // 开枪玩家 ID
        public Vector3 position;    // 子弹初始位置
        public Vector3 forward;     // 子弹运动方向
        public float speed;         // 子弹初速
        public int element;         // 子弹元素
        public float baseHit;           // 基础伤害
        public float headHit;           // 爆头伤害
        public float elementMeasure;    // 元素量
        public float moveTime;  // 最长运动时间
        public float clientTime;     // 发射时间戳
    }
}