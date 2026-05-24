using UnityEngine;

namespace mWeapon.ComponentData
{
	/// <summary>
    /// 武器位置数据
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Component/Transform")]
    public class TransformComponentData : WeaponComponentData
    {
        [Header("武器初始位置")]
        public Vector3 pos;

        [Header("武器初始旋转")] 
        public Vector3 rot;
    }
}