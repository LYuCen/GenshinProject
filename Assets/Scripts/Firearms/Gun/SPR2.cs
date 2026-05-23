using System;
using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using Connect;
using Connect.Converter;
using Connect.Data;
using Firearms.Magazine;
using GameFSM.FSM;
using GameFSM.FSM.WeaponFSM.SPR2;
using GameFSM.LevelFSM.PlayerFSM;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BulletSystem;
using mSystem.SoundSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Firearms.Gun
{
    public class SPR2 : AbstractGun, IWeapon
    {
        private Dictionary<string, AnimationClip> _animDict = new();
        public PlayerManager Manager { get; set; }
        private BaseFsm<AbstractGun> _fsm;
        
        public void Init()
        {
            UniqueId = $"{GetType().Name}";
            _fsm = new Spr2Fsm(new Spr2Builder(this));
            Fsm = _fsm;
            FsmDebuggerRegistry.Register(UniqueId, this);
            
            Magazine = new BaseMagazine(0, 10, 80, ElementEnum.Fire);
            IsReload = false;
            IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/ScriptableObject/Weapon/SPR2.asset", false);
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
            if(!Muzzle) Muzzle = Gun.Find("Muzzle");
            if(!MagazineTransform) MagazineTransform = Gun.Find("Magazine");
            if (!Camera) Camera = Gun.Find("Camera");
            if (!Hand) Hand = Gun.Find("hand");
            Gun.localPosition = new Vector3(0.0801f,-0.0018f,-0.0046f);
            Gun.localRotation = Quaternion.Euler(20.4752f, 82.8698f, 203.5212f);
        }

        public void SetEnable(bool enable)
        {
            Enable = enable;
        }
        
        public override void Shoot()
        {
            if (IsBolt) return;
            IsBolt = true;
            IsShoot = false;
            // 准备原始数据
            ShootData data = new ShootData
            {
                shooterId = NetworkService.Bridge.GetMyId(),
                position = Muzzle.position,
                forward = (Manager.cameraController.rayEnd - Muzzle.position).normalized,
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
            muzzleObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            Muzzle.Muzzle muzzle = muzzleObj.GetComponent<Muzzle.Muzzle>();
            muzzle.UpdateColor(ElementModel.Instance.colors[data.element]);

            // 加入子弹系统
            BulletSystem.Instance.EnQueue(bullet);

            // 弹药逻辑
            if (data.shooterId == UserClient.MyId)
                Magazine.BulletNumber--;
        }

        public void OnPrimaryAction()
        {
            IsShoot = !IsBolt && !IsReload;
            Debug.Log($"[SPR2] OnPrimaryAction - HashCode:{this.GetHashCode()}, IsShoot:{IsShoot}, IsReload:{IsReload}");
        }

        public void PressPrimaryAction()
        {
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
            if (!Gun)
            {
                IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Weapon/SPR2.prefab", false);
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
            AudioSystem.Instance.PlaySound(Config.getWeaponAudioClip);
            Fsm = _fsm;
            FsmDebuggerRegistry.Register(UniqueId, this);
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