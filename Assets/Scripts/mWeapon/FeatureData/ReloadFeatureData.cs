using UnityEngine;

namespace mWeapon.FeatureData
{
    /// <summary>
    /// 武器换弹数据
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Feature/Reload")]
    public class ReloadFeatureData : WeaponFeatureData
    {
        [Header("换弹时长")]
        public float reloadDuration;
    }
}