using AssetBundleFramework.Core.Resource;
using Firearms.Magazine;
using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Query;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BulletSystem;
using mSystem.SoundSystem;
using UnityEngine;

namespace Firearms.Gun
{
    public class XM7 : AbstractGun, IWeapon
    {
        public PlayerManager Manager { get; set; }
        public void Init()
        {
            IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Weapon/Gun/XM7.prefab", false);
            GameObject prefab = ir.GetAsset<GameObject>();
            Gun = Object.Instantiate(prefab).transform;
            Muzzle = Gun.Find("Muzzle");
            MagazineTransform = Gun.Find("Magazine");
            Magazine = new BaseMagazine(0, 30, 30, ElementEnum.Fire);
            IsReload = false;
            ir = ResourceManager.instance.Load("Assets/AssetBundle/ScriptableObject/Weapon/XM7.asset", false);
            Config = ir.GetAsset<WeaponGunConfig>();
        }

        public void InitTransform()
        {
            Gun.localPosition = new Vector3(0.00058f, 0.00174f, -0.0003f);
            Gun.localEulerAngles = new Vector3(305.455383f, 185.162918f, 1.81413245f);
        }

        public void TryReload(int element)
        {
            if(IsReload || (Magazine.BulletNumber == Magazine.BulletCapacity && element == (int)Magazine.Element)) return;
            AudioSystem.Instance.PlaySound(Config.reloadAudioClip);
            IsReload = true;
        }

        public override void Shoot()
        {
            
            GameObject obj = BulletGameObjectPool.Instance.GetObj();
            obj.transform.position = Muzzle.position;
            obj.transform.forward = GunForward;
            Bullet.Bullet bullet = new Bullet.Bullet()
            {
                Transform = obj.transform,
                baseHit = Magazine.BaseHurt * Config.hitMp,
                element = (int)Magazine.Element,
                headHit = Magazine.BaseHurt * Config.headshotMp,
                elementMeasure = 1.5f,
                moveNormalize = GunForward,
                speed = Config.bulletSpeed,
                MoveTime = 3f
            };
            var ps = bullet.Transform.GetChild(0).GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = ElementModel.Instance.colors[bullet.element];
            BulletSystem.Instance.EnQueue(bullet);
            GameObject muzzleObj = FireMuzzleObjectPool.Instance.GetObj();
            muzzleObj.transform.SetParent(Muzzle);
            muzzleObj.gameObject.SetActive(true);
            muzzleObj.transform.localPosition = Vector3.zero;
            muzzleObj.transform.localEulerAngles = new Vector3(90, 0, 0);
            muzzleObj.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            Muzzle.Muzzle muzzle = muzzleObj.GetComponent<Muzzle.Muzzle>();
            muzzle.UpdateColor(ElementModel.Instance.colors[(int)Magazine.Element]);
            AudioSystem.Instance.PlaySound(Config.shootAudioClip);
            //Magazine.BulletNumber--;
        }

        public void OnPrimaryAction()
        {
        }

        public void PressPrimaryAction()
        {
            Shoot();
        }

        public void UpPrimaryAction()
        {
            
        }

        public void OnSecondaryAction()
        {
        }

        public void PressSecondaryAction()
        {
            
        }

        public void UpSecondaryAction()
        {
            
        }

        public void ReloadAction(AbstractMagazine magazine)
        {
            ReloadEnd(magazine);
        }

        public float ReloadTime()
        {
            return 0f;
        }

        public AbstractMagazine GetMagazine()
        {
            return Magazine;
        }

        public void UpdateForward(Vector3 forward)
        {
            GunForward = forward;
        }

        public void UpdateWeapon()
        {
            UpdateState();
        }

        public Transform GetWeapon()
        {
            return Gun;
        }
        
        public void SetEnable(bool enable)
        {
            Enable = enable;
        }
        
        public bool GetEnable()
        {
            return Enable;
        }
        
        public void OnWeaponSwitchEnter()
        {
            AudioSystem.Instance.PlaySound(Config.getWeaponAudioClip);
        }

        public void OnWeaponSwitchExit()
        {
            Gun.gameObject.SetActive(false);
        }

        public AnimationClip GetClipByTag(string tag)
        {
            throw new System.NotImplementedException();
        }

        public WeaponQuery GetQuery()
        {
            throw new System.NotImplementedException();
        }
    }
}