using UnityEngine;

namespace mWeapon.FeatureData
{
    /// <summary>
    /// 武器姿态
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Feature/Stance")]
    public class StanceFeatureData : WeaponFeatureData
    {
        [Header("进入肩射时间")]
        public float shoulderThreshold = 0.2f;
    }
}