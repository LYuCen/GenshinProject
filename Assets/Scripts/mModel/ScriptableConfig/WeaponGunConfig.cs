using System.Collections.Generic;
using mEnum;
using Player.AnimPro;
using UnityEngine;

namespace mModel.ScriptableConfig
{
    [System.Serializable]
    public struct TagClipPair
    {
        public string tag;         // 对应编辑器里的 Lookup Tag，如 "Reload"
        public AnimationClip clip; // 对应的动画文件
    }

    
    [CreateAssetMenu(menuName = "Config/WeaponConfig")]
    public class WeaponGunConfig : ScriptableObject
    {
        public WeaponType weaponType;
        public string weaponName; // 武器唯一标识名
        [Tooltip("子弹初速")] public float bulletSpeed; // 子弹初速
        [Tooltip("击中倍率")] public float hitMp; // 击中倍率
        [Tooltip("爆头倍率")] public float headshotMp; // 爆头倍率
        [Tooltip("射击间隔")] public float shootCd; // 射击间隔
        [Tooltip("换弹时间")] public float reloadCd; // 换弹时间
        [Tooltip("换弹声音")] public AudioClip reloadAudioClip;
        [Tooltip("射击声音")] public AudioClip shootAudioClip;
        [Tooltip("切换声音")] public AudioClip getWeaponAudioClip;
        
        [Header("动画树")]
        public AnimGraphData wholeTreeGraph; 
        [Header("动画标签映射")]
        public List<TagClipPair> animMappings = new List<TagClipPair>();

        // 辅助方法：通过 Tag 查找 Clip
        public AnimationClip FindClip(string tag)
        {
            foreach (var pair in animMappings)
            {
                if (pair.tag == tag) return pair.clip;
            }
            return null;
        }
    }
}