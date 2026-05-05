using Connect.Data;
using Connect.DTO;
using Factory;
using Firearms;
using Firearms.ThrowWeapon;
using Firearms.WeaponInterface;
using GameFSM.LevelFSM.PlayerFSM;
using mSystem.BuffSystem.ElementBuff;
using mSystem.InputSystem.mEvent;
using mView;
using Player.ManagerLogic;
using QFramework;
using UnityEngine;

namespace mController
{
    public class FirearmController : MonoBehaviour,IController,ICanSendEvent
    {
        [SerializeField] private int bulletIndex;
        [SerializeField] private float shootTime;
        [SerializeField] private int[] bullets = new int[8];
        [SerializeField] private WeaponInfoView view;
        public IWeapon Weapon;
        public int weaponIndex = -1; // 当前武器下标
        private IWeapon[] _weaponSlots = new IWeapon[3]; // 三个武器槽： 主武器1， 主武器2， 道具
        private WeaponStateData[] _weaponStates = new WeaponStateData[3];
        private Grenade _grenade;
        public bool weaponChangeEnable;
        public bool isReadyThrow;
        public bool isDownThrow;
        public bool isThrow;
        public bool isOnPrimaryAction;
        public float currentActionNormalizedTime = 0f;

        [SerializeField] private Transform hand;
        [SerializeField] private ElementEnum firearmElement;
        [SerializeField] private int bulletNum;
        [SerializeField] private float reloadTime;
        [SerializeField] private PlayerManager myManager;
        [SerializeField] private Transform muzzle;
        public Transform weaponPivot;
        
        private WeaponFactory _factory;

        public void Init()
        {
            _factory = new WeaponFactory(myManager);
            
            bulletIndex = 1;
            weaponIndex = -1;
            weaponChangeEnable = true;
            AddBullet(90, ElementEnum.Fire);
            AddBullet(90, ElementEnum.Elect);
            AddBullet(90, ElementEnum.Grass);
            
            
            _weaponSlots[2] = _factory.GetWeapon("Grenade");
            _weaponSlots[2].Manager = myManager;
            _weaponSlots[2].GetWeapon().SetParent(hand);
            _weaponSlots[2].InitTransform();
            _weaponSlots[2].GetWeapon().gameObject.SetActive(false);
            
            view?.UpdateText(0, bullets[bulletIndex]);
            view?.Init(bullets);
        }

        public void UserThrow()
        {
            GetWeapon(2); 
        }

        public void UpdateForward(Vector3 forward)
        {
            Weapon.UpdateForward(forward);
        }

        public void ChangeElement(float scroll)
        {
            int res = 0;
            if (scroll > 0) res = -1;
            else if (scroll < 0) res = 1;
            view?.UpdateElementBullets(res);
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
                string defaultName = (index == 0) ? "HK416" : "MK14";
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
                myManager.bodyIk.solver.leftHandEffector.target = gun.Hand;
                if (myManager.cameraController && myManager.GetLogic() is LocalPlayerLogic)
                {
                    myManager.cameraController.scopeCamera = gun.Camera.transform;
                }
            }
            _factory.SyncWeaponAnimation(Weapon); // 同步动画

            Weapon.OnWeaponSwitchEnter();
            Weapon.GetWeapon().gameObject.SetActive(true);
            weaponIndex = index;

            var config = Weapon.GetWeaponConfig();
            view?.UpdateWeapon(config.weaponName, index);
            view?.UpdateText(Weapon.GetMagazine().GetBulletNumber(), bullets[bulletIndex]);
            view?.ElementImage(Weapon.GetMagazine().GetElementEnum());

            return Weapon.GetWeapon();
        }

        public void AddBullet(int bulletNumber, ElementEnum element)
        {
            bullets[(int)element] += bulletNumber;
        }

        public void DownPrimaryAction() // 按下武器主功能键
        {
            //Debug.Log("按下武器主功能键");
            Weapon.OnPrimaryAction();

        }
        public void PressPrimaryAction(bool isNetworkSync = false, ShootDto dto = null) 
        {
            
            if (Weapon is AbstractThrow)
            {
                Weapon.PressPrimaryAction(); // 此时执行本地的 Shoot()
                
                return;
            }
            // 本地玩家逻辑
            if (Weapon.GetMagazine().BulletNumber > 0)
            {
                firearmElement = Weapon.GetMagazine().Element;
                Weapon.PressPrimaryAction(); // 此时执行本地的 Shoot()
            }
            else
            {
                this.SendEvent(new ReloadEvent()); // 只有本地玩家子弹空了才换弹
            }
    
            view?.UpdateText(Weapon.GetMagazine().BulletNumber, bullets[bulletIndex]);
        }

        public void UpPrimaryAction() // 抬起武器主功能键
        {
            //Debug.Log("抬起武器主功能键");
            Weapon.UpPrimaryAction();
        }

        public void DownSecondaryAction(ref bool isShoot)
        {
            Weapon.OnSecondaryAction(ref isShoot);
            view.EnableFrontSight(!isShoot);
        }

        public void PressSecondaryAction()
        {
            myManager.isShoulder = true;
            view.EnableFrontSight(true);
        }

        public void UpSecondaryAction()
        {
            if (myManager.isShoulder) myManager.IsShoot = false;
            myManager.isShoulder = false;
        }

        public bool TryReload()
        {
            if (!view) return false;
            if (Weapon is IReloadable reloadable)
            {
                ElementEnum elementEnum = view.GetChose();
                return reloadable.TryReload(elementEnum, bullets[(int)elementEnum]);
            }
            view.EnableFrontSight(true);
            return false;
        }

        public void Reload()
        {
            if (!view) return;
            if (Weapon is IReloadable reloadable)
            {
                bulletIndex = (int)view.GetChose();
                reloadable.ReloadEnd((ElementEnum)bulletIndex, ref bullets);
            
                // 更新子弹面板数据
                view.UpdateText(Weapon.GetMagazine().BulletNumber, bullets[bulletIndex]);
                if(weaponIndex == -1) view.GrenadeElementImage(bulletIndex);
                else view.ElementImage(bulletIndex);
                view.UpdateBulletsText(bullets);
            }
        }

        private void Update()
        {
            bulletNum = Weapon.GetMagazine().BulletNumber;
            reloadTime = Weapon.ReloadTime();
            Weapon.UpdateWeapon();
                        
            if (weaponIndex == 2 && _weaponSlots[2] != null)
            {
                if (_weaponSlots[2] is AbstractThrow t)
                {
                    isDownThrow = t.GetIsDownThrow();
                    isReadyThrow = t.GetIsReadyThrow();
                    isThrow = t.GetIsThrow();
                    isOnPrimaryAction = t.GetIsOnPrimaryAction();
                }
            }
        }

        public Grenade GetGrenade()
        {
            if (_weaponSlots[2] is Grenade g)
            {
                return g;
            }

            return null;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}