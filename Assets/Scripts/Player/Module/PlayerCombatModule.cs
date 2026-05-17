using System;
using Firearms;
using mEnum.Flags;
using mEnum.Query;
using mEvent;
using mModel;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BulletSystem;
using Player.Context;
using QFramework;
using UnityEngine;

namespace Player.Module
{
    public class PlayerCombatModule : IModule, ICanSendEvent, ICanGetModel
    {
        private CombatContext _context;
        private WeaponQuery _query;
        private bool _isReload;     // 换弹
        private bool _isThrow;      // 投掷
        private bool _isShoot;      // 射击
        private bool _isShoulder;   // 肩射
        private bool _isBolt;       // 拉栓
        private AbstractGun _gun;
        private BagModel _bagModel;
        
        public PlayerCombatModule(CombatContext context)
        {
            _context = context;
        }
        
        public void Init()
        {
            _query = new WeaponQuery();
            _bagModel = this.GetModel<BagModel>();
            GetPrimaryWeapon();
            BulletGameObjectPool.Instance.Init(20);
        }

        public void OnUpdate()
        {
            if(_context.InputContext.PrimaryAction.Down) _context.FirearmController.DownPrimaryAction();
            if (_context.InputContext.PrimaryAction.Press) _context.FirearmController.PressPrimaryAction();
            if(_context.InputContext.PrimaryAction.Up) _context.FirearmController.UpPrimaryAction();
        }

        public void OnFixedUpdate()
        {
            
        }
        
        public void ResetGun(IWeapon weapon)
        {
            if (weapon == null) return;
            weapon.GetWeapon().SetParent(_context.GunCube, true); // 保持世界位置/旋转/缩放
            weapon.InitTransform();
        }

        public void TryReload(ElementEnum chose, int bullet, Action action)
        {
            _isReload = _context.FirearmController.TryReload(chose, bullet);
            _isShoot = _isShoot && !_isReload;
            if(!_isShoot) this.SendEvent(new ExitShootEvent());
            if (_isReload && _context.FirearmController.Weapon is AbstractGun gun)
            {
                gun.StartControlledAction(action);
            }
        }
        
        public void ReloadEnd()
        {
            _isReload = false;
            _context.FirearmController.Reload();
        }

        public void GetPrimaryWeapon()
        {
            _isReload = false;
            _isThrow = false;
            _context.Weapon = _context.FirearmController.GetWeapon(0);
            Debug.Log(_context.Weapon == null);
            if(!_context.Weapon) Debug.Log("武器为空");
            _context.Weapon.gameObject.SetActive(true);
            _context.FirearmController.Weapon.InitTransform();
            if (_context.FirearmController.Weapon is AbstractGun gun)
            {
                _gun = gun;
                this.SendEvent(new SetHandIkTargetEvent
                {
                    Trans = gun.Hand
                });
                _bagModel.SetPrimaryWeapon(gun);
            }
            else _gun = null;
        }

        public void GetSecondaryWeapon()
        {
            _isReload = false;
            _isThrow = false;
            _context.Weapon = _context.FirearmController.GetWeapon(1);
            if(!_context.Weapon) Debug.Log("武器为空");
            _context.Weapon.gameObject.SetActive(true);
            _context.FirearmController.Weapon.InitTransform();
            if (_context.FirearmController.Weapon is AbstractGun gun)
            {
                _gun = gun;
                this.SendEvent(new SetHandIkTargetEvent
                {
                    Trans = gun.Hand
                });
                _bagModel.SetSecondaryWeapon(gun);
            }
            else _gun = null;
        }

        public WeaponQuery GetQuery()
        {
            _query.SetState(0);
            if(_isReload) _query.AddFlags(WeaponFlags.Reload);
            if(_isThrow) _query.AddFlags(WeaponFlags.Throw);
            if(_isShoulder) _query.AddFlags(WeaponFlags.Shoulder);
            if(_isShoot) _query.AddFlags(WeaponFlags.Shoot);
            if(_gun != null && _gun.IsBolt) _query.AddFlags(WeaponFlags.Blot);
            return _query;
        }

        public void DownSecondaryAction()
        {
            if (_isReload) return;
            _isShoot = !_isShoot;
            _context.FirearmController.DownSecondaryAction();
            if(_isShoot) this.SendEvent(new EnterShootEvent());
            else this.SendEvent(new ExitShootEvent());
        }

        public void PressSecondaryAction()
        {
            if(!_isShoulder) this.SendEvent(new ExitShootEvent());
            _isShoulder = true;
            _context.FirearmController.PressSecondaryAction();
        }

        public void UpSecondaryAction()
        {
            if (_isShoulder) _isShoot = false;
            _isShoulder = false;
            _context.FirearmController.UpSecondaryAction();
        }

        public void DownAbility1()
        {
            _isThrow = true;
            _isShoot = false;
            _context.FirearmController.UserThrow();
        }

        public Transform GetWeapon()
        {
            return _context.Weapon;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}