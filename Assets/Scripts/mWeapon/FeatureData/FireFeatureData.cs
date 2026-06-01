using UnityEngine;

namespace mWeapon.FeatureData
{
    /// <summary>
    /// 武器开火数据
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Feature/Fire")]
    public class FireFeatureData : WeaponFeatureData
    {
        [Header("射速(每分钟)")]
        public float fireRate;
        [Header("伤害")]
        public int damage;
        [Header("元素量")]
        public int measure = 1;
        [Header("枪口初速")]
        public int muzzleVelocity;
        [Header("全/半 自动")]
        public bool automatic;
    }
}