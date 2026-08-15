using UnityEngine;

namespace GameFSM.LevelFSM.LocomotionFSM
{
    public class Locomotion
    {
        // 按键输入
        public bool IsInputMove;
        public bool IsMoveForward;
        public bool JumpDown;
        public bool SprintDown;
        public bool CrouchDown;
        public Vector2 InputMoveVec;
        
        // 运行状态
        public float LevelSpeed; // 水平速度
        public float VerticalSpeed; // 垂直速度
        public bool IsSprint;
        public bool IsCrouch;
        public bool IsJump;
        public Vector3 MoveVec; // 移动正方向
        
        // 物理模拟
        public bool IsGround;
        public float Yaw;
        public float Pitch;
    }
}