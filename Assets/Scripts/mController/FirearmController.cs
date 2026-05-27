using System;
using Factory;
using Firearms;
using Firearms.ThrowWeapon;
using Firearms.WeaponInterface;
using GameFSM.LevelFSM.PlayerFSM;
using mSystem.BuffSystem.ElementBuff;
using Player.Context;
using Player.ManagerLogic;
using UnityEngine;

namespace mController
{
    public class FirearmController : MonoBehaviour
    {
        [SerializeField] private int bulletIndex;
        [SerializeField] private float shootTime;
        public IWeapon Weapon;
        public int weaponIndex = -1; // 当前武器下标
        private IWeapon[] _weaponSlots = new IWeapon[3]; // 三个武器槽： 主武器1， 主武器2， 道具
        private Grenade _grenade;
        public bool isDownThrow;
        public float currentActionNormalizedTime = 0f;

        [SerializeField] private Transform hand;
        [SerializeField] private PlayerManager myManager;
        [SerializeField] private Transform muzzle;
        public Transform weaponPivot;
        
        private WeaponFactory _factory;
        private PlayerContext _context;

        public void Init(PlayerContext context)
        {
            _factory = new WeaponFactory(myManager);
            _context = context;
            
            bulletIndex = 1;
            weaponIndex = -1;
            
            _weaponSlots[2] = _factory.GetWeapon("Grenade");
            _weaponSlots[2].Manager = myManager;
            _weaponSlots[2].GetWeapon().SetParent(hand);
            _weaponSlots[2].InitTransform();
            _weaponSlots[2].GetWeapon().gameObject.SetActive(false);
        }

        public void UserThrow()
        {
            GetWeapon(2); 
        }

        public void UpdateForward(Vector3 forward)
        {
            Weapon.UpdateForward(forward);
        }

        public Transform GetWeapon(int index)
        {
            if (weaponIndex == index) return null;
            
            // 旧武器退出
            Weapon?.OnWeaponSwitchExit();
            Weapon?.GetWeapon().gameObject.SetActive(false);
            
            if (_weaponSlots[index] == null)
            {
                // 这里可以根据 index 决定默认给什么枪，或者通过外部捡枪逻辑传入 Data
                string defaultName = (index == 0) ? "FN_IWS" : "SPR2";
                _weaponSlots[index] = _factory.GetWeapon(defaultName);
                _weaponSlots[index].GetWeapon().SetParent(myManager.gunCube, false);
                _weaponSlots[index].InitTransform();
                _weaponSlots[index].Manager = myManager;
            }
            
            
            Weapon = _weaponSlots[index];
            if (Weapon is AbstractGun gun)
            {
                myManager.aimIK.solver.transform = gun.GetMuzzle();
                myManager.aimIK.solver.axis = Vector3.forward; 
                myManager.aimIK.solver.Initiate(myManager.aimIK.transform);
                if (myManager.cameraController && myManager.GetLogic() is LocalPlayerLogic)
                {
                    myManager.cameraController.scopeCamera = gun.Camera.transform;
                }
            }
            _factory.SyncWeaponAnimation(Weapon); // 同步动画

            Weapon.OnWeaponSwitchEnter();
            Weapon.GetWeapon().gameObject.SetActive(true);
            weaponIndex = index;
            
            _weaponSlots[index].InitTransform();
            return Weapon.GetWeapon();
        }

        public void DownPrimaryAction() // 按下武器主功能键
        {
            Weapon.OnPrimaryAction();
        }
        
        // 按住主功能键
        public void PressPrimaryAction() 
        {
            if (Weapon is AbstractThrow)
            {
                Weapon.PressPrimaryAction(); // 此时执行本地的 Shoot()
            
                return;
            }
            // 本地玩家逻辑
            if (Weapon.GetMagazine().BulletNumber > 0)
            {
                Weapon.PressPrimaryAction(); // 此时执行本地的 Shoot()
            }
        }

        public void UpPrimaryAction() // 抬起武器主功能键
        {
            Weapon.UpPrimaryAction();
        }

        public void DownSecondaryAction()
        {
            Weapon.OnSecondaryAction();
        }

        public void PressSecondaryAction()
        {
            Weapon.PressSecondaryAction();
        }

        public void UpSecondaryAction()
        {
            Weapon.UpSecondaryAction();
        }

        public bool TryReload(ElementEnum chose, int bullet)
        {
            if (Weapon.ReloadTime() > 0) return false;
            if (Weapon is IReloadable reloadable)
            {
                return reloadable.TryReload(chose, bullet);
            }
            return false;
        }

        public void Reload()
        {
            if (Weapon is IReloadable reloadable)
            {
                reloadable.ReloadEnd((ElementEnum)bulletIndex);
            }
        }

        private void Update()
        {
            //Weapon.UpdateWeapon();
                        
            if (weaponIndex == 2 && _weaponSlots[2] != null)
            {
                if (_weaponSlots[2] is AbstractThrow t)
                {
                    isDownThrow = t.GetIsDownThrow();
                }
            }
        }
    }
}