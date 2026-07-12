using AssetBundleFramework.Core.Resource;
using Connect.Data;
using mEnum.Flags;
using mEnum.Query;
using mEvent;
using mInterface;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BulletSystem;
using mWeapon;
using mWeapon.Component;
using mWeapon.Presenter;
using Player.AnimTree.UpperBodyNodes;
using Player.Context;
using QFramework;
using UnityEngine;

namespace Player.Module
{
    public class PlayerCombatModule : IModule, ICanSendEvent, ICanGetModel, ICombatQueryProvider
    {
        private CombatContext _context;
        private WeaponQuery _query;
        private BagModel _bagModel;
        public WeaponPresenter RemoteWeapon;
        private bool _enable;
        
        public PlayerCombatModule(CombatContext context)
        {
            _context = context;
        }
        
        public void Init()
        {
            _query = new WeaponQuery();
            if (_context.IsLocal)
            {
                _bagModel = this.GetModel<BagModel>();
                GetPrimaryWeapon();
            }
            _enable = true;
        }

        public void OnUpdate()
        {
            // 非主控玩家直接跳过
            if (!_context.IsLocal || !_enable) return;
            
            WeaponAction action = new WeaponAction
            {
                Primary = _context.InputContext.PrimaryAction,
                Secondary = _context.InputContext.SecondaryAction,
                Reload = _context.InputContext.Reload
            };
            _bagModel.Weapon?.HandleAction(action);
            _bagModel.Weapon?.Tick();
        }

        public void OnFixedUpdate()
        {
            
        }

        public void ResetGun()
        {
            if (!_context.IsLocal)
            {
                if (RemoteWeapon) RemoteWeapon.transform.SetParent(_context.GunCube, true);
                return;
            }
            Transform weapon = _bagModel.Weapon?.Presenter?.transform;
            if (!weapon) return;
            weapon.SetParent(_context.GunCube, true);       
            _bagModel.Weapon.Presenter.ResetTrans();  
        }

        public void GetPrimaryWeapon()
        {
            _bagModel.Weapon?.Presenter?.OnExit();
            _bagModel.SetPrimaryWeapon();
            if (!_bagModel.Weapon.Presenter)
            {
                IResource ir = ResourceManager.instance.Load($"Assets/AssetBundle/Model/Weapon/{_bagModel.Weapon.WeaponName}.prefab", false);
                GameObject prefab = Object.Instantiate(ir.GetAsset<GameObject>(), _context.GunCube, false);
                _bagModel.Weapon.Presenter = prefab.GetComponent<WeaponPresenter>();
            }
            ActivateWeapon();
        }

        public void GetSecondaryWeapon()
        {
            _bagModel.Weapon?.Presenter?.OnExit();
            _bagModel.SetSecondaryWeapon();
            if (!_bagModel.Weapon.Presenter)
            {
                IResource ir = ResourceManager.instance.Load($"Assets/AssetBundle/Model/Weapon/{_bagModel.Weapon.WeaponName}.prefab", false);
                GameObject prefab = Object.Instantiate(ir.GetAsset<GameObject>(), _context.GunCube, false);
                _bagModel.Weapon.Presenter = prefab.GetComponent<WeaponPresenter>();
            }
            ActivateWeapon();
        }

        private void ActivateWeapon()
        {
            // 动画树切换
            var animComp = _bagModel.Weapon.RequireComponent<AnimComponent>();
            _context.AnimController.SwitchWeaponGraph(
                animComp.AnimGraphData,
                _bagModel.Weapon.WeaponType
            );
            TagClipPair tagClipPair = new TagClipPair();
            for (int i = 0; i < animComp.AnimMappings.Count; i++)
            {
                if (animComp.AnimMappings[i].tag == "Reload")
                {
                    tagClipPair = animComp.AnimMappings[i];
                    break;
                }
            }
            _context.AnimController.InstallSlotAction("UpperSlot", manager =>
            {
                var node = new ReloadTreeLeafNode(manager);
                node.AnimSpeed = tagClipPair.animSpeed;
                node.Tag = tagClipPair.tag;
                node.UseTagOnly = false;
                node.DefaultClip = tagClipPair.clip;
                node.CurrentData = tagClipPair.clipAsset;
                return node;
            });
            _bagModel.Weapon.Presenter.gameObject.SetActive(true);
            this.SendEvent(new SetHandIkTargetEvent
            {
                Trans = _bagModel.Weapon.Presenter.hand,
                Muzzle = _bagModel.Weapon.Presenter.muzzle
            });
            _bagModel.Weapon.Presenter.Init(_bagModel.Weapon, _context.CameraController);
            _bagModel.Weapon.Presenter.OnEnter();
            _context.CameraController.scopeCamera = _bagModel.Weapon.Presenter.gunSight;
            _context.CameraController.scopeCamera?.gameObject.SetActive(false); // 初始隐藏，开镜时才显
        }

        public WeaponQuery GetQuery()
        {
            _query.SetState(0);
            if (!_enable) return _query;
            if (_context.IsLocal)
            {
                if(_bagModel.Weapon.Runtime.HasState(WeaponStateTags.Reloading)) _query.AddFlags(WeaponFlags.Reload);
                if(_bagModel.Weapon.Runtime.Stance == WeaponStance.Shoulder) _query.AddFlags(WeaponFlags.Shoulder);
                if(_bagModel.Weapon.Runtime.Stance == WeaponStance.Ads) _query.AddFlags(WeaponFlags.Shoot);
                if(_bagModel.Weapon.Runtime.HasState(WeaponStateTags.Bolting)) _query.AddFlags(WeaponFlags.Bolt);
            }
            else
            {
                var state = _context.StateData;
                if(state.isShoot) _query.AddFlags(WeaponFlags.Shoot);
                if(state.isReload) _query.AddFlags(WeaponFlags.Reload);
                if(state.isThrow) _query.AddFlags(WeaponFlags.Throw);
            }
            return _query;
        }
        
        /// <summary>
        /// 网络同步状态
        /// </summary>
        public void SyncFromState(PlayerStateData state)
        {
            _context.StateData.isShoot = state.isShoot;
            _context.StateData.isThrow = state.isThrow;
            _context.StateData.isReload = state.isReload;
        }

        /// <summary>
        /// 写入网络状态
        /// </summary>
        public void WriteToState(PlayerStateData state)
        {
            var query = GetQuery();
            state.isShoot = query.HasState(WeaponFlags.Shoot);
            state.isReload = query.HasState(WeaponFlags.Reload);
            state.isThrow = query.HasState(WeaponFlags.Throw);
        }

        public void DownSecondaryAction()
        {
            
        }

        public void PressSecondaryAction()
        {
            
        }

        public void UpSecondaryAction()
        {
            
        }

        public void DownAbility1()
        {
            
        }

        public Transform GetWeapon()
        {
            if (_context.IsLocal) return _bagModel.Weapon?.Presenter?.transform;
            return RemoteWeapon.transform;
        }

        public void SetEnable(bool enable)
        {
            _enable = enable;
        }

        public void Reset()
        {
            if (_context.IsLocal)
            {
                _bagModel.Weapon.Runtime.Reset();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}