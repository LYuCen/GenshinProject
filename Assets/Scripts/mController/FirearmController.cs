using Common.FSM.PlayerFSM;
using Firearms;
using Firearms.Gun;
using Firearms.ThrowWeapon;
using Firearms.WeaponInterface;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BulletSystem;
using mSystem.InputSystem.mEvent;
using mView;
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
        public int weaponIndex; // 当前武器下标
        private IWeapon _mainWeapon1; // 主武器1
        private IWeapon _mainWeapon2; // 主武器2
        private IWeapon _throwWeapon; // 投掷武器
        private Grenade _grenade;
        public bool weaponChangeEnable;
        public bool isReadyThrow;
        public bool isDownThrow;

        [SerializeField] private Transform hand;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject muzzlePrefab;
        [SerializeField] private GameObject grenadePrefab;
        [SerializeField] private ElementEnum firearmElement;
        [SerializeField] private int bulletNum;
        [SerializeField] private float reloadTime;
        [SerializeField] private PlayerManager myManager;

        private int _gunBulletIndex;

        private void Start()
        {
            bulletIndex = 1;
            weaponIndex = -1;
            weaponChangeEnable = true;
            AddBullet(90, ElementEnum.Fire);
            AddBullet(90, ElementEnum.Elect);
            AddBullet(90, ElementEnum.Water);
            
            BulletGameObjectPool.Instance.Init(bulletPrefab);
            FireMuzzleObjectPool.Instance.Init(muzzlePrefab);
            GrenadeObjectPool.Instance.Init(grenadePrefab);
            view.UpdateText(0, bullets[bulletIndex]);
            view.Init(bullets);
            
            // 初始化投掷物
            _grenade = new Grenade();
            _throwWeapon = _grenade;
            _throwWeapon.Init();
            Transform trans = _throwWeapon.GetWeapon();
            trans.SetParent(hand);
            _throwWeapon.InitTransform();
            trans.gameObject.SetActive(false);
        }

        public void UserThrow()
        {
            Weapon.GetWeapon().gameObject.SetActive(false);
            _throwWeapon.GetWeapon().gameObject.SetActive(true);
            weaponIndex = -1;
            Weapon = _throwWeapon;
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
            view.UpdateElementBullets(res);
        }
        
        public void AwakeGun(IWeapon weapon)
        {
            Weapon = weapon;
            
            view.UpdateText(Weapon.GetMagazine().GetBulletNumber(), bullets[bulletIndex]);
            view.ElementImage(Weapon.GetMagazine().GetElementEnum());
        }

        public Transform GetWeapon(int index)
        {
            Transform trans = null;

            if (index == 0 && weaponIndex != index)
            {
                if (_mainWeapon1 == null)
                {
                    _mainWeapon1 = new FN_IWS();
                    _mainWeapon1.Init();
                }
                view.UpdateWeapon("FN IWS",0);
                Weapon?.OnWeaponSwitchExit();
                AwakeGun(_mainWeapon1);
                Weapon?.OnWeaponSwitchEnter();
                trans = _mainWeapon1.GetWeapon();
            }
            else if(index == 1 && weaponIndex != index)
            {
                if (_mainWeapon2 == null)
                {
                    _mainWeapon2 = new MK14();
                    _mainWeapon2.Init();
                }
                view.UpdateWeapon("MK14",1);
                Weapon?.OnWeaponSwitchExit();
                AwakeGun(_mainWeapon2);
                Weapon?.OnWeaponSwitchEnter();
                trans = _mainWeapon2.GetWeapon();
            }

            if (trans)
            {
                weaponIndex = index;
                _throwWeapon.GetWeapon().gameObject.SetActive(false);
            }
            return trans;
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
        
        public void PressPrimaryAction() // 按住武器主功能键
        {
            //Debug.Log("按住武器主功能键");
            if (Weapon.GetMagazine().BulletNumber > 0)
            {
                firearmElement = Weapon.GetMagazine().Element;
                Weapon.PressPrimaryAction();
            }
            else
            {
                this.SendEvent(new ReloadEvent());
            }
            view.UpdateText(Weapon.GetMagazine().BulletNumber, bullets[bulletIndex]);
        }

        public void UpPrimaryAction() // 抬起武器主功能键
        {
            //Debug.Log("抬起武器主功能键");
            Weapon.UpPrimaryAction();
        }

        public void DownSecondaryAction(ref bool isShoot)
        {
            Weapon.OnSecondaryAction(ref isShoot);
        }

        public bool TryReload()
        {
            if (Weapon is IReloadable reloadable)
            {
                ElementEnum elementEnum = view.GetChose();
                return reloadable.TryReload(elementEnum, bullets[(int)elementEnum]);
            }

            return false;
        }

        public void Reload()
        {
            if (Weapon is IReloadable reloadable)
            {
                bulletIndex = (int)view.GetChose();
                reloadable.ReloadEnd((ElementEnum)bulletIndex, ref bullets);
                
                _gunBulletIndex = bulletIndex;
            
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
            
                        
            if (weaponIndex == -1)
            {
                isDownThrow = _grenade.GetIsDownThrow();
                isReadyThrow = _grenade.GetIsReadyThrow();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}