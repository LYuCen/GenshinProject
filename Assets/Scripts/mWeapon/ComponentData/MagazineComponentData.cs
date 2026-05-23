using mSystem.BuffSystem.ElementBuff;
using UnityEngine;
using UnityEngine.Serialization;

namespace mWeapon.ComponentData
{
    /// <summary>
    /// 武器弹匣数据
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Component/Magazine")]
    public class MagazineComponentData : WeaponComponentData
    {
        [Header("弹匣元素")]
        public ElementEnum magazineElement;
        [Header("弹匣容量")] 
        public int magazineCapacity;
    }
}