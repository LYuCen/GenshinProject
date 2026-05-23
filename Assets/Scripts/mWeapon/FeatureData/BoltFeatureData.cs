using UnityEngine;

namespace mWeapon.FeatureData
{
    /// <summary>
    /// 武器拉栓数据
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Feature/Bolt")]
    public class BoltFeatureData : WeaponFeatureData
    {
        [Header("拉栓时长")]
        public float boltDuration;
    }
}