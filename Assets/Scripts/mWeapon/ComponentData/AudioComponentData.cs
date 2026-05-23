using UnityEngine;

namespace mWeapon.ComponentData
{
    /// <summary>
    /// 武器部位伤害数据
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Component/Audio")]
    public class AudioComponentData : WeaponComponentData
    {
        [Header("切换音效")]
        public AudioClip getAudioClip;
        [Header("开火音效")]
        public AudioClip fireAudioClip;
        [Header("换弹音效")]
        public AudioClip reloadAudioClip;
        [Header("拉栓音效")] 
        public AudioClip boltAudioClip;
    }
}