using System.Collections.Generic;
using mWeapon.FeatureData;
using UnityEngine;

namespace mWeapon
{
    [CreateAssetMenu(menuName = "Weapon/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName;
        public List<WeaponFeatureData> features;
        public List<WeaponComponentData> components;
    }
}