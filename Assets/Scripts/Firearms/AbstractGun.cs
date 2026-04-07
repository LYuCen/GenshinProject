using Connect.DTO;
using Firearms.WeaponInterface;
using mModel;
using mModel.ScriptableConfig;
using mSystem.BuffSystem.ElementBuff;
using mSystem.SoundSystem;
using UnityEngine;

namespace Firearms
{
    /// <summary>
    /// 枪械抽象类
    /// </summary>
    public abstract class AbstractGun : IReloadable
    {
        protected AbstractMagazine Magazine; // 弹匣
        protected Transform Gun; // 枪
        protected Transform Muzzle;
        protected Transform MagazineTransform;
        protected Vector3 GunForward;
        protected WeaponGunConfig Config;
        protected float PrevShootTime; // 上次射击时间
        protected bool IsCanShoot; // 是否可以射击
        protected float EndReloadTime; // 换弹结束时间
        protected bool IsReload; // 是否正在换弹
        protected bool Enable = true;

        protected void UpdateState()
        {
            IsReload = EndReloadTime > Time.time;
            IsCanShoot = PrevShootTime + Config.shootCd < Time.time && !IsReload;
        }

        
        public abstract void Shoot();

        public virtual void RemoteShoot(ShootDto dto) { }

        public WeaponGunConfig GetWeaponConfig()
        {
            return Config;
        }

        public bool TryReload(ElementEnum ele, int bullet)
        {
            int index = (int)ele;
            // 子弹已上满并且上的是同元素的子弹
            if (Magazine.IsFull() && Magazine.GetElementEnum() == index) return false;
            // 子弹数量为 0
            if (bullet <= 0) return false;
            
            // 进入换弹阶段 播放声音
            SoundSystem.Instance.PlaySound(Config.reloadAudioClip);
            IsCanShoot = false;
            IsReload = true;
            EndReloadTime = Time.time + Config.reloadCd;
            return true;
        }
        
        public bool TryReload(AbstractMagazine magazine)
        {
            if (magazine == null) return false;
            
            // 进入换弹阶段 播放声音
            SoundSystem.Instance.PlaySound(Config.reloadAudioClip);
            IsCanShoot = false;
            IsReload = true;
            EndReloadTime = Time.time + Config.reloadCd;
            return true;
        }
        
        public void ReloadEnd(ElementEnum ele, ref int[] bullets)
        {
            bullets[Magazine.GetElementEnum()] += Magazine.GetBulletNumber();
            Magazine.SetBulletNumber(bullets[(int)ele], ele);
            bullets[Magazine.GetElementEnum()] -= Magazine.GetBulletNumber();
            
            // 更新武器材质
            if (MagazineTransform)
            {
                MeshRenderer meshRenderer = MagazineTransform.GetComponent<MeshRenderer>();
                Material[] materials = meshRenderer.materials;
                int m = materials.Length;
                materials[m - 1] = ElementModel.Instance.materials[Magazine.GetElementEnum()];
                meshRenderer.materials = materials;
            }
        }
        
        public void ReloadEnd(AbstractMagazine magazine)
        {
            Magazine = magazine;
            
            // 更新武器材质
            if (MagazineTransform)
            {
                MeshRenderer meshRenderer = MagazineTransform.GetComponent<MeshRenderer>();
                Material[] materials = meshRenderer.materials;
                int m = materials.Length;
                materials[m - 1] = ElementModel.Instance.materials[Magazine.GetElementEnum()];
                meshRenderer.materials = materials;
            }
        }
    }
}
