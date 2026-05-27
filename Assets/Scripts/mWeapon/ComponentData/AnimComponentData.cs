using System.Collections.Generic;
using mModel.ScriptableConfig;
using Player.AnimPro;
using UnityEngine;

namespace mWeapon.ComponentData
{
    /// <summary>
    /// 武器动画数据
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Component/Anim")]
    public class AnimComponentData : WeaponComponentData
    {
        [Header("动画树")]
        public AnimGraphData animGraphData;
        [Header("特殊动画数据")]
        public List<TagClipPair> animMappings;
    }
}