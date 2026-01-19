using UnityEngine;

namespace mModel.ScriptableConfig
{
    [CreateAssetMenu(menuName = "Config/WeaponConfig")]
    public class WeaponGunConfig : ScriptableObject
    {
        [Tooltip("子弹初速")] public float bulletSpeed; // 子弹初速
        [Tooltip("击中倍率")] public float hitMp; // 击中倍率
        [Tooltip("爆头倍率")] public float headshotMp; // 爆头倍率
        [Tooltip("射击间隔")] public float shootCd; // 射击间隔
        [Tooltip("换弹时间")] public float reloadCd; // 换弹时间
        [Tooltip("换弹动画")] public AnimConfig reloadAnimConfig;
        [Tooltip("换弹声音")] public AudioClip reloadAudioClip;
        [Tooltip("射击声音")] public AudioClip shootAudioClip;
        [Tooltip("切换声音")] public AudioClip getWeaponAudioClip;
    }
}