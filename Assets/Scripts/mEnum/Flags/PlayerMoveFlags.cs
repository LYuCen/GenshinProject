namespace mEnum.Flags
{
    [System.Flags]
    public enum PlayerMoveFlags
    {
        /// <summary> 空状态 </summary>
        None = 0, 
        /// <summary> 前 </summary>
        Forward = 1 << 0,
        /// <summary> 右前 </summary>
        ForwardRight = 1 << 1,
        /// <summary> 右 </summary>
        Right = 1 << 2,
        /// <summary> 右后 </summary>
        BackRight = 1 << 3,
        /// <summary> 后 </summary>
        Back = 1 << 4,
        /// <summary> 左后 </summary>
        BackLeft = 1 << 5,
        /// <summary> 左 </summary>
        Left = 1 << 6,
        /// <summary> 左前 </summary>
        ForwardLeft = 1 << 7,
        /// <summary> 地面 </summary>
        Ground = 1 << 8,
        /// <summary> 跳跃 </summary>
        Jump = 1 << 9,
        /// <summary> 冲刺 </summary>
        Sprint = 1 << 10,
        /// <summary> 蹲 </summary>
        Crouch = 1 << 11,
        /// <summary> 滑铲 </summary>
        Sliding = 1 << 12,
        /// <summary> 移动 </summary>
        Move = 1 << 13,
    }
}