using Connect;
using Connect.Converter;
using Connect.Simulation;
using Firearms;
using GameFSM.LevelFSM.PlayerFSM;
using mController.Player.InputController;
using mEnum.Flags;
using mEnum.Query;
using mEvent;
using mModel;
using mSystem.BuffSystem.ElementBuff;
using mSystem.InputSystem.Context;
using QFramework;
using UnityEngine;

namespace Player.ManagerLogic
{
    public class LocalPlayerLogic : IPlayerLogic, IController
    {
        private PlayerManager _m;
        private Vector3 _bottom;
        private float _stableTimer; // 输入状态切换当前时间
        
        private InputContext _context;
        private PlayerInputController _inputController;
        private IUnRegister _mReloadEvent;
        private IUnRegister _mSetHandEvent;
        
        public void Init(PlayerManager manager, InputContext context = null)
        {
            _m = manager;
            _context = context;
            _inputController = new PlayerInputController(_m);
            if (NetworkService.Bridge is LocalServerSimulator lss)
            {
                lss.InitPlayerManager(manager);
            }

            _mReloadEvent = this.RegisterEvent<ReloadEvent>(_ => TryReload());
            _mSetHandEvent = this.RegisterEvent<SetHandIkTargetEvent>(e => InitHandIK(e.Trans));
        }

        public void OnUpdate()
        {
            _inputController.Process(_context);

            _m.state.aimPosition = _m.cameraController.rayEnd;

            _m.firearmController.UpdateForward((_m.cameraController.rayEnd - _m.gunStock.position).normalized);
            _m.AnimController.Update();
            SendPositionToServer();
            if(_context.Reload.Down) TryReload();
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
            WeaponQuery query = _m.CombatModule.GetQuery();
            if (query.HasState(WeaponFlags.Reload)) return;

            int chose = _m.ViewModule.GetChose();
            BulletInfo info = _m.BagModule.GetBulletInfo(chose);
            _m.CombatModule.TryReload(info.Data, info.Val, () => Reload(info.Data, info.Val));
        }

        private void Reload(ElementEnum e, int val)
        {
            AbstractMagazine magazine = _m.firearmController.Weapon.GetMagazine();
            _m.BagModule.ReturnBullet(new BulletInfo { Data = magazine.Element, Val = magazine.BulletNumber });
            val = Mathf.Min(val, magazine.BulletCapacity);
            magazine.BulletNumber = val;
            magazine.Element = e;
            _m.BagModule.DeductBullet(new BulletInfo { Data = magazine.Element, Val = magazine.BulletNumber });
            _m.CombatModule.ReloadEnd();
        }

        private void InitHandIK(Transform hand)
        {
            _m.IkModule.SetHandIk(hand);
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