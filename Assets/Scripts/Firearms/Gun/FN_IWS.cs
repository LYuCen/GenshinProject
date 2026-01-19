using Firearms.Magazine;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BulletSystem;
using mSystem.SoundSystem;
using UnityEngine;

namespace Firearms.Gun
{
    public class FN_IWS : AbstractGun, IWeapon
    {
        public void Init()
        {
            Gun = Object.Instantiate(Resources.Load<Transform>("Prefab/FN IWS"));
            Muzzle = Gun.Find("Muzzle");
            MagazineTransform = Gun.Find("Magazine");
            Magazine = new BaseMagazine(0, 30, 30, ElementEnum.Fire);
            PrevShootTime = 0;
            IsCanShoot = true;
            IsReload = false;
            Config = Resources.Load<WeaponGunConfig>("WeaponConfig/FN_IWS");
        }

        public void InitTransform()
        {
            Gun.transform.localPosition = new Vector3(-0.076f, -1.722f, -0.203f);
        }

        public void TryReload(int element)
        {
            if(IsReload || (Magazine.BulletNumber == Magazine.BulletCapacity && element == (int)Magazine.Element)) return;
            SoundSystem.Instance.PlaySound(Config.reloadAudioClip);
            IsCanShoot = false;
            IsReload = true;
            EndReloadTime = Time.time + Config.reloadCd;
        }

        public void SetEnable(bool enable)
        {
            Enable = enable;
        }

        public override void Reload(AbstractMagazine magazine)
        {
            Magazine = magazine;
            // 更新武器材质
            MeshRenderer meshRenderer = MagazineTransform.GetComponent<MeshRenderer>();
            Material[] materials = meshRenderer.materials;
            int m = materials.Length;
            materials[m - 1] = ElementModel.Instance.materials[(int)magazine.Element];
            meshRenderer.materials = materials;
        }

        public override void Shoot()
        {
            if (!IsCanShoot) return;
            IsCanShoot = false;
            PrevShootTime = Time.time;
            
            GameObject obj = BulletGameObjectPool.Instance.GetObj();
            obj.transform.position = Muzzle.position;
            obj.transform.forward = GunForward;
            Bullet.Bullet bullet = new Bullet.Bullet()
            {
                Transform = obj.transform,
                baseHit = Magazine.BaseHurt * Config.hitMp,
                element = (int)Magazine.Element,
                headHit = Magazine.BaseHurt * Config.headshotMp,
                elementMeasure = 1f,
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
            SoundSystem.Instance.PlaySound(Config.shootAudioClip);
            Magazine.BulletNumber--;
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

        public void ReloadAction(AbstractMagazine magazine)
        {
            Reload(magazine);
        }

        public float ReloadTime()
        {
            return Mathf.Max(0, EndReloadTime - Time.time);
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
        
        public bool GetEnable()
        {
            return Enable;
        }

        public void OnWeaponSwitchEnter()
        {
            SoundSystem.Instance.PlaySound(Config.getWeaponAudioClip);
        }

        public void OnWeaponSwitchExit()
        {
            
        }
    }
}