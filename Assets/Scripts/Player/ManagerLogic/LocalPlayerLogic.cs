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
using mWeapon.ComponentData;
using Player.AnimTree;
using Player.Module;
using QFramework;
using UnityEngine;

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
            SendPositionToServer();
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