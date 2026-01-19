using mModel.ScriptableConfig;
using UnityEngine;

namespace Firearms
{
    public abstract class AbstractGun // 枪械抽象类
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
        
        public abstract void Reload(AbstractMagazine magazine);
        public abstract void Shoot();

        public WeaponGunConfig GetWeaponConfig()
        {
            return Config;
        }

        public virtual void OnDisable()
        {
            Gun.gameObject.SetActive(false);
            foreach (Transform info in Muzzle)
            {
                FireMuzzleObjectPool.Instance.RemoveObj(info.gameObject);
            }
        }
    }
}
