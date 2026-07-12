using System;
using Connect;
using Connect.Simulation;
using Connect.Sync;
using GameFSM.LevelFSM.PlayerFSM;
using mController.Player.InputController;
using mEvent;
using mModel;
using mSystem.InputSystem;
using mSystem.InputSystem.Context;
using mView.BulletChose;
using Player.Module;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using MessageType = Connect.Protocol.MessageType;
using Object = UnityEngine.Object;

namespace Player.ManagerLogic
{
    public class LocalPlayerLogic : IPlayerLogic, IController
    {
        private PlayerManager _m;
        private Vector3 _bottom;
        private float _stableTimer; // 输入状态切换当前时间
        
        
        private PlayerInputController _inputController;
        private IUnRegister _mSetHandEvent;
        private BagModel _bagModel;
        
        private InputContext _context;
        private IInputSystem _inputSystem;

        private float _moveTimer;
        private float _boneTimer;

        private int _lastFlags = -1;
        private AnnulusView _annulusView; // 轮盘选择
        private float _timer;

        private PlayerSyncManager _syncManager;
        
        public void Init(PlayerManager manager)
        {
            Debug.Log("创建本地玩家");
            _m = manager;
            _bagModel = this.GetModel<BagModel>();
            _context = new InputContext();
            _inputSystem = this.GetSystem<IInputSystem>();
            _inputSystem.InitHandler(_context);
            
            manager.CombatContext.InputContext = _context;
            manager.CombatModule = new PlayerCombatModule(manager.CombatContext);
            manager.MoveContext.CombatModule = manager.CombatModule;
            manager.MoveModule = new PlayerMoveModule(manager.MoveContext);
            manager.ViewModule = new PlayerViewModule(manager.PlayerContext);
            manager.BagModule = new PlayerBagModule(manager.PlayerContext);
            manager.IkModule = new PlayerIkModule(manager.IKContext);
            
            _inputController = new PlayerInputController(_m);
            if (NetworkService.Bridge is LocalServerSimulator lss)
            {
                lss.InitPlayerManager(manager);
            }
            _mSetHandEvent = this.RegisterEvent<SetHandIkTargetEvent>(InitHandIK);
            
            manager.BagModule.Init();
            manager.CombatModule.Init();
            manager.MoveModule.Init();
            manager.IkModule.Init();
            manager.ViewModule.Init();
            
            _annulusView = Object.FindObjectOfType<AnnulusView>(true); // 场景里找        
            if (_annulusView)
            {
                int count = _bagModel.BulletCount;
                _annulusView.Init(count, 1);
                for (int i = 0; i < count; i++)
                {
                    var bullet = _bagModel.GetBullet(i);
                    var img = _annulusView.icons[i].GetComponent<Image>();
                    if (img && ElementModel.Instance) img.sprite = ElementModel.Instance.elementUI[(int)bullet.Data];  
                }
                _annulusView.HideWheel();
            }

            _syncManager = new PlayerSyncManager();
            _syncManager.Register(new MoveSyncChannel(_m.EntityId));
            _syncManager.Register(new AnimSyncChannel(_m.EntityId, _bagModel));
        }

        public void OnUpdate()
        {
            if (_inputSystem.GamePlayEnable)
                _inputController.Process(_context);
            else
                _m.MoveModule.InputMove(Vector2.zero);

            _m.state.aimPosition = _m.cameraController.rayEnd;

            _m.AnimController.Update();
            
            int currentId = NetworkService.Bridge.GetMyId();
            if (currentId != -1)
            {
                _m.state.playerId = currentId;
                _m.state.position = _m.transform.position;  
                _m.state.rotation = _m.transform.rotation;  
                _m.MoveModule.WriteToState(_m.state);       
                _m.CombatModule.WriteToState(_m.state);   
                CheckState();
            }
            
            Reload();
            
            _m.ViewModule?.OnUpdate();
            _m.CombatModule?.OnUpdate();
            _m.MoveModule?.OnUpdate();
            _m.IkModule?.OnUpdate();
            
            _syncManager.OnFixedUpdate(_m);
        }

        public void OnFixedUpdate()
        {
            int currentId = NetworkService.Bridge.GetMyId();
            if (currentId == -1) return;

            // 20 tick: 位置+旋转+输入
            /*
            _moveTimer += Time.fixedDeltaTime;
            if (_moveTimer >= 0.05f)
            {
                _moveTimer -= 0.05f;
                NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_Move, PlayerDataConverter.PackMove(_m.state)));
            }

            // 20 tick: 骨骼
            _boneTimer += Time.fixedDeltaTime;
            if (_boneTimer >= 0.05f)
            {
                _boneTimer -= 0.05f;
                _m.state.clientTime = Time.time;
                NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_AnimationState,
                    PlayerDataConverter.PackAnimationState(_m.EntityId, _m.MoveModule.GetQuery().State,
                        _m.CombatModule.GetQuery().State, (sbyte)_bagModel.Weapon.InternalRuntime.Stance,
                        _m.cameraController.rayEnd, Time.time, _m.state.clientTime)));
            }
            */

        }

        private void Reload()
        {
            if (_context.Reload.Down)
            {
                _timer = 0;
            }

            if (_context.Reload.Press)
            {
                _timer += Time.deltaTime;
                if (_timer > 0.2f && !_annulusView.gameObject.activeSelf)
                {
                    _annulusView.ShowWheel();
                }

                if (_annulusView.gameObject.activeSelf)
                {
                    int idx = _annulusView.GetIndexByMouse(Input.mousePosition);
                    _annulusView.SetHighLight(idx);
                    if (idx >= 0)
                    {
                        _m.BagModule.SetChose(idx);
                    } 
                }
            }
            if (_context.Reload.Up)
            {
                _annulusView.HideWheel();
            }
        }

        public void OnLateUpdate()
        {
            _m.cameraController.UpdateForward();
        }

        private void InitHandIK(SetHandIkTargetEvent e)
        {
            _m.IkModule.SetHandIk(e.Trans);
            _m.IkModule.SetMuzzle(e.Muzzle);
        }

        private void CheckState()
        {
            int currentFlags = 0;
            if (_m.state.isGround)  currentFlags |= PlayerStateBoolBit.BIT_GROUND;
            if (_m.state.isJump)    currentFlags |= PlayerStateBoolBit.BIT_JUMP;
            if (_m.state.isShoot)   currentFlags |= PlayerStateBoolBit.BIT_SHOOT;
            if (_m.state.isReload)  currentFlags |= PlayerStateBoolBit.BIT_RELOAD;
            if (_m.state.isSprint)  currentFlags |= PlayerStateBoolBit.BIT_SPRINT;
            if (_m.state.isThrow)   currentFlags |= PlayerStateBoolBit.BIT_THROW;
            if (_m.state.isCrouch)  currentFlags |= PlayerStateBoolBit.BIT_CROUCH;
            if (_m.state.isSliding) currentFlags |= PlayerStateBoolBit.BIT_SLIDING;

            // 状态变化时发送
            if (currentFlags != _lastFlags)
            {
                _lastFlags = currentFlags;
                byte[] flagData = BitConverter.GetBytes(currentFlags);
                NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_StateChange, flagData));
            }
        }

        public void InitState()
        {
            int currentFlags = 0;
            if (_m.state.isGround)  currentFlags |= PlayerStateBoolBit.BIT_GROUND;
            if (_m.state.isJump)    currentFlags |= PlayerStateBoolBit.BIT_JUMP;
            if (_m.state.isShoot)   currentFlags |= PlayerStateBoolBit.BIT_SHOOT;
            if (_m.state.isReload)  currentFlags |= PlayerStateBoolBit.BIT_RELOAD;
            if (_m.state.isSprint)  currentFlags |= PlayerStateBoolBit.BIT_SPRINT;
            if (_m.state.isThrow)   currentFlags |= PlayerStateBoolBit.BIT_THROW;
            if (_m.state.isCrouch)  currentFlags |= PlayerStateBoolBit.BIT_CROUCH;
            if (_m.state.isSliding) currentFlags |= PlayerStateBoolBit.BIT_SLIDING;
            
            _lastFlags = currentFlags;
            byte[] flagData = BitConverter.GetBytes(currentFlags);
            NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_StateChange, flagData));
        }

        public void UnRegister()
        {
            _mSetHandEvent.UnRegister();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}