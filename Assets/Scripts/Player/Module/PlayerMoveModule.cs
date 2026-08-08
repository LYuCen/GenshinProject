using AI;
using AI.VisualRequest.VisualRequest;
using Connect.Data;
using mEnum.Flags;
using mEnum.Query;
using mInterface;
using mSystem.InputSystem.Context;
using Player.Context;
using UnityEngine;

namespace Player.Module
{
    public class PlayerMoveModule : IModule, IMoveQueryProvider
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
        
        private float _gravity = -8f; // 重力加速度
        private float _terminalVelocity = -10f; // 终端速度
        private Vector3 _bottom;

        private PointVisualRequest _point = new ();

        public Vector2 GetMoveInput => _move;

        private bool _enable;

        private FsmContext _fsmContext;
        public PlayerMoveModule(MoveContext context)
        {
            _context = context;
        }
        
        public void Init()
        {
            VisualizerManager.Instance.Register(_point);
            _enable = true;
        }

        public void InitFsmContext(FsmContext context)
        {
            _fsmContext = context;
        }

        public void OnUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            if (!_enable) return;
            _moveSpeed = _fsmContext.Locomotion.LevelSpeed;
            _isSprint = _fsmContext.Locomotion.IsSprint;
            _isJump = _fsmContext.Locomotion.IsJump;
            _isCrouch = _fsmContext.Locomotion.IsCrouch;
            _move = _fsmContext.Locomotion.InputMoveVec;
            float targetYaw = _context.CameraController.Yaw;
            _fsmContext.Locomotion.Yaw = targetYaw;
            
            // Quaternion.Euler 为旋转矩阵  Quaternion.Euler(0, targetYaw, 0) 是向量为 y轴旋转 targetYaw度
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move = forward * _move.y + right   * _move.x;

            Vector3 horizontalMove;
            if (_isGround)
            {
                horizontalMove = move.normalized * (_moveSpeed * 0.02f);
            }
            else
            {
                horizontalMove = _fsmContext.Locomotion.MoveVec.normalized * (_moveSpeed * 0.02f);
            }
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            _context.PlayerTransform.rotation = Quaternion.Slerp(
                _context.PlayerTransform.transform.rotation,
                targetRot,
                0.02f * 10f
            );
            _context.CharacterController.Move(
                horizontalMove +
                Vector3.up * (_fsmContext.Locomotion.VerticalSpeed * 0.02f)
            );
            
            CheckGrounded();
            OnDrawGizmos();
            ApplyGravity();
        }

        public void Reset()
        {
            _move = Vector2.zero;
            _isSprint = false;
            _isCrouch = false;
            _isJump = false;
            _isGround = false;
            _moveSpeed = 0f;
        }

        public void InputMove(Vector2 vec)
        {
            _move = vec;
        }

        /// <summary>
        /// 网络同步状态
        /// </summary>
        public void SyncFromState(PlayerStateData state)
        {
            _isGround  = state.isGround;
            _isJump    = state.isJump;
            _isSprint  = state.isSprint;
            _isCrouch  = state.isCrouch;
            _isSliding = state.isSliding;
            _move      = state.inputMove;
        }

        /// <summary>
        /// 写入网络状态
        /// </summary>
        public void WriteToState(PlayerStateData state)
        {
            state.isGround  = _isGround;
            state.isJump    = _isJump;
            state.isSprint  = _isSprint;
            state.isCrouch  = _isCrouch;
            state.isSliding = _isSliding;
            state.inputMove = _move;
        }
        
        private Vector3 _groundCheckOrigin;
        private float _groundCheckRadius;
        private bool _groundHit;
        
        private void CheckGrounded()
        {
            _groundCheckOrigin = _context.PlayerTransform.position + Vector3.up * 0.2f;
            
            _groundCheckRadius = _context.CharacterController.radius * 0.9f;
            
            _groundHit = false;

            if (Physics.SphereCast(_groundCheckOrigin, _groundCheckRadius, Vector3.down, out RaycastHit hit, 0.2f))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up); // 计算夹角

                if (angle <= 45f)
                {
                    _groundHit = true;
                    _isGround = true;
                    _fsmContext.Locomotion.IsGround = true;
                    return;
                }
            }
            _isGround = false;
            _fsmContext.Locomotion.IsGround = false;
        }
        
        private void OnDrawGizmos()
        {
            if (_context.CharacterController == null) return;
            _point.PointPos = _groundCheckOrigin;
            _point.Radius = _groundCheckRadius;
            _point.PointColor = Color.white;
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
                if (_fsmContext.Locomotion.VerticalSpeed < 0) 
                {
                    _fsmContext.Locomotion.VerticalSpeed = -2.0f; 
                }
            }
            else
            {
                // 不在地面时，累加重力加速
                _fsmContext.Locomotion.VerticalSpeed += _gravity * 0.02f;
        
                // 限制最大下落速度
                if (_fsmContext.Locomotion.VerticalSpeed < _terminalVelocity) 
                {
                    _fsmContext.Locomotion.VerticalSpeed = _terminalVelocity;
                }
            }
        }
        
        /// <summary>
        /// 获取状态
        /// </summary>
        public PlayerMoveQuery GetQuery()
        {
            _query.SetState(0);
            if (!_enable) return _query;
            
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

        public void SetEnable(bool enable)
        {
            _enable = enable;
        }
    }
}