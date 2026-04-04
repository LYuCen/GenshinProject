namespace Connect.DTO
{
    [System.Serializable]
    public class ShootDto
    {
        public int shooterId;       // 开枪玩家 ID
        public float px, py, pz;    // 子弹初始位置坐标
        public float fx, fy, fz;    // 子弹 forward 方向
        public float speed;         // 子弹初速
        public int element;         // 子弹元素
        public float baseHit;           // 基础伤害
        public float headHit;           // 爆头伤害
        public float elementMeasure;    // 元素量
        public float moveTime;  // 最长运动时间
        public long serverTick;     // 发射时间戳
    }
}