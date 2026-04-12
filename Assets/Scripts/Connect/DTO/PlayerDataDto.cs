namespace Connect.DTO
{
    [System.Serializable]
    public class PlayerDataDto
    {
        public int playerId;
        public int allBoolState;
        
        // 使用平坦字段代替数组，消除 GC
        // 每个 float[] 都是一个堆对象. 一次 ToDto 产生 5 个数组对象
        public float px, py, pz; // Position
        public float rx, ry, rz, rw; // Rotation (Quaternion)
        public float ax, ay, az; // AimPosition
        public float ix, iy; // InputMove
        public float mx, my; // PlayerMove
        
        public float curHp;     // 当前血量
        public float maxHp;     // 最大血量
        public float curShield; // 当前盾量
        public float maxShield; // 最大盾量
        
        // Hips 骨骼的局部坐标 (Local Position)
        public float hx, hy, hz; 
        public ushort[] bRots; // 骨骼旋转
    }
}