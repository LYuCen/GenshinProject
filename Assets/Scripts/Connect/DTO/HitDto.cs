namespace Connect.DTO
{
    [System.Serializable]
    public class HitDto
    {
        public int attackerId;      // 攻击者ID
        public int victimId;        // 受害者ID（需要从碰撞体获取）
        public int damage;          // 最终伤害量
        public int hitType;         // 0:身体, 1:头部
        public int element;         // 元素
        public float elementMeasure;    // 元素量
    }
}