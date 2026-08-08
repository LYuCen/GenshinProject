using GameFSM.LevelFSM.LocomotionFSM;
using UnityEngine;

namespace Server
{
    public class ServerMoveModule
    {
        private Locomotion _locomotion;
        private float _gravity = -8f;
        private float _terminalVelocity = -10f;
        private CharacterController _player;

        public ServerMoveModule(Locomotion locomotion, CharacterController player)
        {
            _locomotion = locomotion;
            _player = player;
        }

        public void Update(float deltaTime)
        {
            float targetYaw = _locomotion.Yaw;
            
            // Quaternion.Euler 为旋转矩阵  Quaternion.Euler(0, targetYaw, 0) 是向量为 y轴旋转 targetYaw度
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move = forward * _locomotion.InputMoveVec.y + right * _locomotion.InputMoveVec.x;

            Vector3 horizontalMove;
            CheckGrounded();
            // 刚离地时快照当前速度
            if (_locomotion.IsGround)
            {
                horizontalMove = move.normalized * (_locomotion.LevelSpeed * deltaTime);
            }
            else
            {
                horizontalMove = _locomotion.MoveVec.normalized * (_locomotion.LevelSpeed * deltaTime);
            }
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            _player.transform.rotation = Quaternion.Slerp(
                _player.transform.rotation,
                targetRot,
                deltaTime * 10f
            );
            _player.Move(
                horizontalMove +
                Vector3.up * (_locomotion.VerticalSpeed * deltaTime)
            );
            ApplyGravity(deltaTime);
        }
        
        private Vector3 _groundCheckOrigin;
        private float _groundCheckRadius;
        
        public void CheckGrounded()
        {
            _groundCheckOrigin = _player.transform.position + Vector3.up * 0.2f;
            
            _groundCheckRadius = _player.radius * 0.9f;

            if (Physics.SphereCast(_groundCheckOrigin, _groundCheckRadius, Vector3.down, out RaycastHit hit, 0.2f))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up); // 计算夹角

                if (angle <= 45f)
                {
                    _locomotion.IsGround = true;
                    return;
                }
            }

            _locomotion.IsGround = false;
        }
        
        private void ApplyGravity(float deltaTime)
        {
            if (_locomotion.IsGround)
            {
                if (_locomotion.VerticalSpeed < 0) 
                {
                    _locomotion.VerticalSpeed = -2.0f; 
                }
            }
            else
            {
                // 不在地面时，累加重力加速
                _locomotion.VerticalSpeed += _gravity * deltaTime;
        
                // 限制最大下落速度
                if (_locomotion.VerticalSpeed < _terminalVelocity) 
                {
                    _locomotion.VerticalSpeed = _terminalVelocity;
                }
            }
        }
    }
}