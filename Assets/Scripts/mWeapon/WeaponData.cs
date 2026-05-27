using System.Collections.Generic;
using mEnum;
using mWeapon.FeatureData;
using UnityEngine;

namespace mWeapon
{
    [CreateAssetMenu(menuName = "Weapon/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName;
        public WeaponType weaponType = WeaponType.Rifle;
        public List<WeaponFeatureData> features;
        public List<WeaponComponentData> components;
    }
}