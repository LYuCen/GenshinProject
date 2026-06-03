using System;
using Connect;
using Connect.Converter;
using Connect.Simulation;
using GameFSM.LevelFSM.PlayerFSM;
using mController.Player.InputController;
using mEvent;
using mModel;
using mSystem.InputSystem;
using mSystem.InputSystem.Context;
using mWeapon;
using mWeapon.Component;
using Player.Module;
using QFramework;
using UnityEngine;
using MessageType = Connect.Protocol.MessageType;

namespace Player.ManagerLogic
{
    public class LocalPlayerLogic : IPlayerLogic, IController
    {
        private PlayerManager _m;
        private Vector3 _bottom;
        private float _stableTimer; // 输入状态切换当前时间
        
        
        private PlayerInputController _inputController;
        private IUnRegister _mReloadEvent;
        private IUnRegister _mSetHandEvent;
        private BagModel _bagModel;
        
        private InputContext _context;
        private IInputSystem _inputSystem;

        private float _moveTimer;
        private float _boneTimer;

        private int _lastFlags = -1;
        
        public void Init(PlayerManager manager)
        {
            Debug.Log("创建本地玩家");
            _m = manager;
            _bagModel = this.GetModel<BagModel>();
            _context = new InputContext();
            _inputSystem = new InputSystem(_context);
            
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
            _mReloadEvent = this.RegisterEvent<ReloadEvent>(_ => TryReload());
            _mSetHandEvent = this.RegisterEvent<SetHandIkTargetEvent>(InitHandIK);
            
            manager.BagModule.Init();
            manager.CombatModule.Init();
            manager.MoveModule.Init();
            manager.IkModule.Init();
            manager.ViewModule.Init();
        }

        public void OnUpdate()
        {
            _inputController.Process(_context);

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

            if(_context.Reload.Down) TryReload();
            
            // 监听输入
            _inputSystem.SetKeyCode();
            
            _m.ViewModule?.OnUpdate();
            _m.CombatModule?.OnUpdate();
            _m.MoveModule?.OnUpdate();
            _m.IkModule?.OnUpdate();
        }

        public void OnFixedUpdate()
        {
            int currentId = NetworkService.Bridge.GetMyId();
            if (currentId == -1) return;

            // 20 tick: 位置+旋转+输入
            _moveTimer += Time.fixedDeltaTime;
            if (_moveTimer >= 0.05f)
            {
                _moveTimer -= 0.05f;
                NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_Move, PlayerDataConverter.PackMove(_m.state)));
            }

            // 10 tick: 骨骼
            _boneTimer += Time.fixedDeltaTime;
            if (_boneTimer >= 0.1f)
            {
                _boneTimer -= 0.1f;
                _m.state.clientTime = Time.time;
                NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_Bones, PlayerDataConverter.PackBones(_m.state)));
            }

        }

        public void OnLateUpdate()
        {
            _m.cameraController.UpdateForward();
        }

        private void TryReload()
        {
            WeaponPro weapon = _bagModel.Weapon;
            if(weapon == null) return;
            if(weapon.Runtime.HasState(WeaponStateTags.Reloading)) return;

            int chose = _m.ViewModule.GetChose();
            BulletInfo info = _m.BagModule.GetBulletInfo(chose);

            // 没有该元素子弹
            if(info.Val <= 0) return;
            MagazineComponent mag = weapon.GetComponent<MagazineComponent>();
            // 已装满且元素相同，不需要换弹
            if(mag.IsFull && mag.ElementEnum == info.Data) return;
            mag.ElementEnum = info.Data;
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
            _mReloadEvent.UnRegister();
            _mSetHandEvent.UnRegister();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}