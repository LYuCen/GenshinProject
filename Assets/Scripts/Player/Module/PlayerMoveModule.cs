using AI;
using AI.VisualRequest.VisualRequest;
using Connect.Data;
using mEnum.Flags;
using mEnum.Query;
using Player.Context;
using UnityEngine;

namespace Player.Module
{
    public class PlayerMoveModule : IModule
    {
        private MoveContext _context;
        private bool _isGround;
        private bool _isJump;
        private bool _isSprint;
        private bool _isCrouch;
        private bool _isSliding;
        private float _moveSpeed;
        private Vector2 _move;
        private PlayerMoveQuery _query;
        
        private float _gravity = -15.0f; // 重力加速度（通常比现实大一点，手感更好）
        private float _terminalVelocity = -30f; // 终端速度（防止无限加速掉落导致穿模）
        private float _verticalVelocity; // 当前垂直速度
        private PointVisualRequest _request = new PointVisualRequest();
        private Vector3 _bottom;
        public PlayerMoveModule(MoveContext context)
        {
            _context = context;
        }
        
        public void Init()
        {
            _request.Radius = 0.05f;
            VisualizerManager.Instance.Register(_request);
        }

        public void OnUpdate()
        {
            float targetYaw = _context.CameraController.Yaw;
            
            // Quaternion.Euler 为旋转矩阵  Quaternion.Euler(0, targetYaw, 0) 是向量为 y轴旋转 targetYaw度
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move =
                forward * _move.y +
                right   * _move.x;
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            _context.PlayerTransform.rotation = Quaternion.Slerp(
                _context.PlayerTransform.transform.rotation,
                targetRot,
                Time.deltaTime * 10f
            );
            
            _context.CharacterController.Move(
                move.normalized * (_moveSpeed * Time.deltaTime)
            );
            CheckGrounded();
            ApplyGravity();
        }

        public void OnFixedUpdate()
        {
            
        }

        public void InputMove(Vector2 vec)
        {
            _move = vec;
        }

        public void SetSpeed(float speed)
        {
            _moveSpeed = speed;
        }

        /// <summary>
        /// 网络同步状态
        /// </summary>
        public void SyncFromState(PlayerStateData state)
        {
            _isCrouch = state.isCrouch;
            _isSprint = state.isSprint;
            _move = state.inputMove;
        }

        public void Sprint()
        {
            PlayerMoveQuery query = GetQuery();
            WeaponQuery weaponQuery = _context.CombatModule.GetQuery();
            // 移动输入不为 0，并且为三个正方向的移动才能进行奔跑
            _isSprint = query.HasState(PlayerMoveFlags.Move) &&
                        (query.HasState(PlayerMoveFlags.Forward) ||
                         query.HasState(PlayerMoveFlags.ForwardLeft) ||
                         query.HasState(PlayerMoveFlags.ForwardRight));
            _isSprint &= !weaponQuery.HasState(WeaponFlags.Shoot);
            _isSprint &= !weaponQuery.HasState(WeaponFlags.Shoulder);
        }

        public void Crouch()
        {
            _isCrouch = !_isCrouch;
        }
        
        private void CheckGrounded()
        {
            _bottom = _context.PlayerTransform.position + Vector3.down * 0f;
            _request.PointPos = _bottom;
            
            Collider[] hits = Physics.OverlapSphere(_bottom, 0.05f);
    
            bool foundPlane = false;
            foreach (var col in hits)
            {
                if (col.gameObject == _context.PlayerTransform.gameObject) continue; 
                if (col.CompareTag("Plane"))
                {
                    foundPlane = true;
                    break; // 找到了就跳出循环
                }
            }
            
            _isGround = foundPlane;
        }
        
        /// <summary>
        /// 重力实现
        /// </summary>
        private void ApplyGravity()
        {
            if (_isGround)
            {
                // 在地面时，不要让速度为 0
                // 给一个微小的向下压力（如 -2.0），能让角色在走斜坡或下楼梯时紧贴地面
                if (_verticalVelocity < 0) 
                {
                    _verticalVelocity = -2.0f; 
                }
            }
            else
            {
                // 不在地面时，累加重力加速
                _verticalVelocity += _gravity * Time.deltaTime;
        
                // 限制最大下落速度
                if (_verticalVelocity < _terminalVelocity) 
                {
                    _verticalVelocity = _terminalVelocity;
                }
            }
        }
        
        /// <summary>
        /// 获取状态
        /// </summary>
        public PlayerMoveQuery GetQuery()
        {
            // 初始化位掩码为 None (0)
            PlayerMoveFlags flags = PlayerMoveFlags.None;

            // 基础布尔状态转换
            if (_isGround)  flags |= PlayerMoveFlags.Ground;
            if (_isJump)    flags |= PlayerMoveFlags.Jump;
            if (_isSprint)  flags |= PlayerMoveFlags.Sprint;
            if (_isCrouch)  flags |= PlayerMoveFlags.Crouch;
            if (_isSliding) flags |= PlayerMoveFlags.Sliding;

            // 移动方向转换
            float x = _move.x;
            float y = _move.y;
            float t = 0.1f; 

            if (y > t) // 前
            {
                if (x > t)       flags |= PlayerMoveFlags.ForwardRight;
                else if (x < -t) flags |= PlayerMoveFlags.ForwardLeft;
                else             flags |= PlayerMoveFlags.Forward;
            }
            else if (y < -t) // 后
            {
                if (x > t)       flags |= PlayerMoveFlags.BackRight;
                else if (x < -t) flags |= PlayerMoveFlags.BackLeft;
                else             flags |= PlayerMoveFlags.Back;
            }
            else // y轴在中间，判定纯左右
            {
                if (x > t)       flags |= PlayerMoveFlags.Right;
                else if (x < -t) flags |= PlayerMoveFlags.Left;
            }

            if (((int)flags & 0xFF) != 0) flags |= PlayerMoveFlags.Move;
            
            _query.SetState((int)flags);
            
            return _query;
        }
    }
}