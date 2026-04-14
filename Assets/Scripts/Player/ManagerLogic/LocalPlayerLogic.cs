using AI;
using AI.VisualRequest.VisualRequest;
using Connect;
using Connect.Converter;
using Connect.Simulation;
using GameFSM.FSM.PlayerFSM;
using mSystem.InputSystem.mEvent;
using mSystem.SoundSystem;
using QFramework;
using UnityEngine;

namespace Player.ManagerLogic
{
    public class LocalPlayerLogic : IPlayerLogic
    {
        private PlayerManager _m;
        private Vector3 _bottom;
        private Vector2 _lastMove; // 输入切换后移动方向
        private float _stableTimer; // 输入状态切换当前时间
        private float _stableThreshold = 0.05f; // 输入状态切换间隔
        private PointVisualRequest _request = new PointVisualRequest();
        
        private float _verticalVelocity; // 当前垂直速度
        private float _gravity = -15.0f; // 重力加速度（通常比现实大一点，手感更好）
        private float _terminalVelocity = -30f; // 终端速度（防止无限加速掉落导致穿模）
        
        public void Init(PlayerManager manager)
        {
            _m = manager;
            // 注册事件
            _m.RegisterEvent<MoveEvent>(e => _m.InputMove = e.MoveVec);
            _m.RegisterEvent<WeaponSecondaryActionEvent>(e =>
            {
                if (e.IsDown)
                {
                    _m.firearmController.DownSecondaryAction(ref _m.state.isShoot);
                    if (_m.IsShoot) _m.IsSprint = false;
                }
            });
            _m.RegisterEvent<WeaponPrimaryActionEvent>(e =>
            {
                _m.IsSprint = false;
                _m.IsShoot = true && !_m.IsThrow;
                if(e.IsDown) _m.firearmController.DownPrimaryAction();
                if(e.IsPress) _m.firearmController.PressPrimaryAction();
                if(e.IsUp) _m.firearmController.UpPrimaryAction();
            });
            _m.RegisterEvent<ReloadEvent>(_ =>
            {
                if(_m.firearmController.Weapon.ReloadTime() > 0) return;
                _m.IsReload = _m.firearmController.TryReload();
            });
            _m.RegisterEvent<MouseScrollEvent>(e =>
            {
                _m.firearmController.ChangeElement(e.Scroll);
            });
            _m.RegisterEvent<ChangeGunEvent>(e =>
            {
                if (_m.firearmController.weaponIndex != e.GunIndex && _m.firearmController.weaponChangeEnable)
                {
                    _m.IsReload = false;
                    _m.IsThrow = false;
                    if(e.GunIndex == 0)
                    {
                        _m.playerGun = _m.firearmController.GetWeapon(0);
                        Debug.Log("切换为主武器1");
                    }
                    else if(e.GunIndex == 1)
                    {
                        _m.playerGun = _m.firearmController.GetWeapon(1);
                        Debug.Log("切换为主武器2");
                    }
                    if(!_m.playerGun) Debug.Log("武器为空");
                    _m.playerGun.gameObject.SetActive(true);
                    _m.SetParent(_m.playerGun,_m.gunCube);
                    _m.firearmController.Weapon.InitTransform();
                }
            });
            _m.RegisterEvent<RunEvent>(_ =>
            {
                if (_m.InputMove.sqrMagnitude > 0.01f)
                {
                    _m.IsSprint = true;
                    _m.IsShoot = false;
                }
            });
            _m.RegisterEvent<GetGrenade>(_ =>
            {
                _m.IsThrow = true;
                _m.IsShoot = false;
                _m.firearmController.UserThrow();
            });
            _m.RegisterEvent<CrouchEvent>(_ =>
            {
                if (_m.IsSliding) return;
                if (_m.IsSprint && !_m.IsCrouch)
                {
                    if(_m.slidingCd == 0) _m.IsSliding = true;
                }
                else _m.IsCrouch = !_m.IsCrouch;
            });

            if (NetworkService.Bridge is LocalServerSimulator lss)
            {
                lss.InitPlayerManager(manager);
            }
            
            // 绘制角色着地判定区
            _request.Radius = manager.radius;
            VisualizerManager.Instance.Register(_request);
        }

        public void OnUpdate()
        {
            if (_m.IsSprint)
            {
                // 如果 inputMove 归零（或接近零），自动退出冲刺状态
                if (_m.InputMove.sqrMagnitude < 0.001f)
                {
                    _m.IsSprint = false;
                }
            }
            
            // 处理重力
            ApplyGravity();
            // 垂直位移
            Vector3 finalMove = Vector3.up * _verticalVelocity;
            _m.characterController.Move(finalMove * Time.deltaTime);

            _m.state.aimPosition = _m.cameraController.rayEnd;
            CheckGrounded();
            PlayerMoveAnimContro();
            _m.firearmController.UpdateForward((_m.cameraController.rayEnd - _m.gunStock.position).normalized);
            _m.AnimController.Update();
            SendPositionToServer();
        }

        public void OnFixedUpdate()
        {
            _m.slidingCd = Mathf.Max(0, _m.slidingCd - Time.fixedDeltaTime);
        }

        public void OnLateUpdate()
        {
            _m.cameraController.UpdateForward();
        }

        /// <summary>
        /// 发给服务端位置信息
        /// </summary>
        private void SendPositionToServer()
        {
            // 从桥接层获取当前 ID (单机时是 1000, 联机时是服务器分配的)
            int currentId = NetworkService.Bridge.GetMyId();
    
            // 如果还没拿到 ID（联机中途），则不发包
            if (currentId == -1) return; 
    
            // 同步状态
            _m.state.playerId = currentId;
            _m.state.position = _m.transform.position;
            _m.state.rotation = _m.transform.rotation;

            NetworkService.Bridge.SendToServer(new Message("UpdatePosition", PlayerDataConverter.ToDto(_m.state)));

        }
        
        void PlayerMoveAnimContro() // 角色输入移动间隔控制
        {
            if (_m.InputMove == _lastMove) // 输入向量和当前向量相同
            {
                _stableTimer += Time.deltaTime;

                if (_stableTimer >= _stableThreshold) // 时间到达间隔是才允许切换移动动画
                {
                    _m.PlayerMove = _m.InputMove; // 输入稳定，更新动画用的向量
                }
            }
            else
            {
                // 输入发生变化：先重置计时器，但不立刻切换动画
                _stableTimer = 0f;
            }

            _lastMove = _m.InputMove;
        }

        private void CheckGrounded()
        {
            _bottom = _m.transform.position + Vector3.down * _m.h;
            _request.PointPos = _bottom;

            Collider[] hits = Physics.OverlapSphere(_bottom, _m.radius);
    
            bool foundPlane = false;
            foreach (var col in hits)
            {
                if (col.gameObject == _m.gameObject) continue; 
                if (col.CompareTag("Plane"))
                {
                    foundPlane = true;
                    break; // 找到了就跳出循环
                }
            }

            _m.IsGround = foundPlane;
        }
        
        /// <summary>
        /// 重力实现
        /// </summary>
        private void ApplyGravity()
        {
            if (_m.IsGround)
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
    }
}