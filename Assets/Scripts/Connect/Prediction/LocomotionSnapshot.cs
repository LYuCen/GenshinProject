using UnityEngine;

namespace Connect.Prediction
{
    /// <summary>
    /// 客户端运动快照
    /// </summary>
    public struct LocomotionSnapshot
    {
        public float LevelSpeed;
        public float VerticalSpeed;
        public bool IsSprint;
        public bool IsCrouch;
        public bool IsJump;
        public bool IsGround;
        public Vector3 MoveVec;
        public Vector2 InputMoveVec;
        public float Yaw;
    }
}