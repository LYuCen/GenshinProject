using AssetBundleFramework.Core.Resource;
using Connect;
using Connect.Converter;
using Connect.Data;
using Connect.DTO;
using Firearms.Magazine;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BulletSystem;
using mSystem.SoundSystem;
using Newtonsoft.Json;
using UnityEngine;

namespace Firearms.Gun
{
    public class FN_IWS : AbstractGun, IWeapon
    {
        public void Init()
        {
            IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Weapon/Gun/FN IWS.prefab", false);
            GameObject prefab = ir.GetAsset<GameObject>();
            Gun = Object.Instantiate(prefab).transform;
            Muzzle = Gun.Find("Muzzle");
            MagazineTransform = Gun.Find("Magazine");
            Magazine = new BaseMagazine(0, 30, 30, ElementEnum.Fire);
            PrevShootTime = 0;
            IsCanShoot = true;
            IsReload = false;
            ir = ResourceManager.instance.Load("Assets/AssetBundle/ScriptableObject/Weapon/FN_IWS.asset", false);
            Config = ir.GetAsset<WeaponGunConfig>();
        }

        public void InitTransform()
        {
            Gun.transform.localPosition = new Vector3(-0.03f, -1.74f, -2.83f);
            Gun.transform.localEulerAngles = new Vector3(274.814636f, 70.0597076f, 286.340332f);
        }

        public void SetEnable(bool enable)
        {
            Enable = enable;
        }

        public override void Shoot()
        {
            if (!IsCanShoot) return;
            IsCanShoot = false;
            PrevShootTime = Time.time;

            ShootData data = new ShootData
            {
                shooterId = NetworkService.Bridge.GetMyId(),
                position = Muzzle.position,
                forward = GunForward,
                speed = Config.bulletSpeed,
                baseHit = Magazine.BaseHurt * Config.hitMp,
                headHit = Magazine.BaseHurt * Config.headshotMp,
                element = (int)Magazine.Element,
                elementMeasure = 1f,
                moveTime = 3f,
                serverTick = (long)Time.time,
            };

            var dto = ShootDataConverter.ToDto(data);
            string json = JsonConvert.SerializeObject(dto);
            NetworkService.Bridge.SendToServer(new Message("PlayerShoot", json));
            ExecuteShoot(dto);
        }
        
        // 新增：远程玩家执行开火的方法
        public void RemoteShoot(ShootDto dto)
        {
            ExecuteShoot(dto);
        }
        
        private void ExecuteShoot(ShootDto dto)
        {
            GameObject obj = BulletGameObjectPool.Instance.GetObj();
            obj.transform.position = new Vector3(dto.px, dto.py, dto.pz);
            Vector3 direction = new Vector3(dto.fx, dto.fy, dto.fz);
            obj.transform.forward = direction;

            Bullet.Bullet bullet = new Bullet.Bullet()
            {
                Transform = obj.transform,
                shooterId = dto.shooterId,
                baseHit = dto.baseHit,
                element = dto.element,
                headHit = dto.headHit,
                elementMeasure = dto.elementMeasure,
                moveNormalize = direction,
                speed = dto.speed,
                MoveTime = dto.moveTime
            };

            // 设置粒子效果
            var ps = bullet.Transform.GetChild(0).GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = ElementModel.Instance.colors[bullet.element];

            // 加入子弹系统移动（这一步所有客户端都会执行，子弹在各自本地飞）
            BulletSystem.Instance.EnQueue(bullet);
            
            SoundSystem.Instance.PlaySound(Config.shootAudioClip);
    
            // 如果是本地玩家，减弹药
            if(dto.shooterId == UserClient.MyId)
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

        public void OnSecondaryAction(ref bool isShoot)
        {
            isShoot = !isShoot;
        }

        public void ReloadAction(AbstractMagazine magazine)
        {
            ReloadEnd(magazine);
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
            Gun.gameObject.SetActive(false);
        }
    }
}