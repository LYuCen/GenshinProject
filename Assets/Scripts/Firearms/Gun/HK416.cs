using System;
using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using Connect;
using Connect.Converter;
using Connect.Data;
using Firearms.Magazine;
using GameFSM.FSM.PlayerFSM;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BulletSystem;
using mSystem.SoundSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Firearms.Gun
{
    public class HK416 : AbstractGun, IWeapon
    {
        private Dictionary<string, AnimationClip> _animDict = new();
        public PlayerManager Manager { get; set; }
        
        public void Init()
        {
            Magazine = new BaseMagazine(0, 30, 30, ElementEnum.Fire);
            PrevShootTime = 0;
            IsCanShoot = true;
            IsReload = false;
            IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/ScriptableObject/Weapon/HK416.asset", false);
            Config = ir.GetAsset<WeaponGunConfig>();
            
            foreach (var pair in Config.animMappings)
            {
                if (!_animDict.ContainsKey(pair.tag))
                    _animDict.Add(pair.tag, pair.clip);
            }
        }

        public void InitTransform()
        {
            if (!Gun) Gun = GetWeapon();
            Muzzle = Gun.Find("Muzzle");
            MagazineTransform = Gun.Find("Magazine");
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

            // 准备原始数据
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
                serverTick = DateTime.UtcNow.Ticks,
            };

            // 使用 FlatBuffers 打包为二进制
            byte[] flatBytes = ShootDataConverter.ToDto(data);

            // 发送二进制 Message (不再需要 JSON)
            NetworkService.Bridge.SendToServer(new Message("PlayerShoot", flatBytes));

            // 本地立即执行开火表现
            ExecuteShoot(data);
        }

        // 供远程玩家调用
        public override void RemoteShoot(byte[] flatData)
        {
            ShootData data = new ShootData();
            ShootDataConverter.FillData(flatData, data);
            ExecuteShoot(data);
        }
        
        // 本地和远程共用
        private void ExecuteShoot(ShootData data)
        {
            GameObject obj = BulletGameObjectPool.Instance.GetObj();
            obj.transform.position = data.position;
            obj.transform.forward = data.forward;

            Bullet.Bullet bullet = new Bullet.Bullet()
            {
                Transform = obj.transform,
                shooterId = data.shooterId,
                baseHit = data.baseHit,
                element = data.element,
                headHit = data.headHit,
                elementMeasure = data.elementMeasure,
                moveNormalize = data.forward,
                speed = data.speed,
                MoveTime = data.moveTime
            };

            // 设置粒子和枪焰
            var ps = bullet.Transform.GetChild(0).GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = ElementModel.Instance.colors[bullet.element];
            
            GameObject muzzleObj = FireMuzzleObjectPool.Instance.GetObj();
            muzzleObj.transform.SetParent(Muzzle);
            muzzleObj.gameObject.SetActive(true);
            muzzleObj.transform.localPosition = Vector3.zero;
            muzzleObj.transform.localEulerAngles = new Vector3(90, 0, 0);
            muzzleObj.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            Muzzle.Muzzle muzzle = muzzleObj.GetComponent<Muzzle.Muzzle>();
            muzzle.UpdateColor(ElementModel.Instance.colors[data.element]);

            // 加入子弹系统
            BulletSystem.Instance.EnQueue(bullet);
            SoundSystem.Instance.PlaySound(Config.shootAudioClip);

            // 弹药逻辑
            if (data.shooterId == UserClient.MyId)
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
            if (!Gun)
            {
                IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Weapon/FN_IWS.prefab", false);
                GameObject prefab = ir.GetAsset<GameObject>();
                Gun = Object.Instantiate(prefab).transform;
            }
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

        public AnimationClip GetClipByTag(string tag)
        {
            if (_animDict.TryGetValue(tag, out var clip))
            {
                return clip;
            }
        
            Debug.LogWarning($"武器 {Config.weaponName} 未配置 Tag 为 {tag} 的动画");
            return null;
        }
    }
}