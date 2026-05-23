using UnityEngine;

namespace mWeapon.ComponentData
{
    /// <summary>
    /// 武器部位伤害数据
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Component/DamageProfile")]
    public class DamageProfileComponentData : WeaponComponentData
    {
        [Header("头部倍率")]
        public float headMultiplier = 1.9f;
        [Header("身体倍率")]
        public float bodyMultiplier = 1.0f;
        [Header("四肢倍率")]
        public float limbMultiplier = 0.4f;
    }
}