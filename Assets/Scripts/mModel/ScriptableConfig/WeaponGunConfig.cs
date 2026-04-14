using mEnum;
using UnityEngine;

namespace mModel.ScriptableConfig
{
    [CreateAssetMenu(menuName = "Config/WeaponConfig")]
    public class WeaponGunConfig : ScriptableObject
    {
        public WeaponType weaponType;
        public string weaponName; // 武器唯一标识名

        [Header("特殊动作 (上半身)")]
        public string reloadAnimConfig;

        [Header("该类型通用移动动作 (下半身)")]
        public string idleAnimPath;
        public string walkAnimPath;
        public string sprintAnimPath;
        public string cIdleAnimPath;
        public string cWalkAnimPath;
        public string cSprintAnimPath;
        
        [Tooltip("子弹初速")] public float bulletSpeed; // 子弹初速
        [Tooltip("击中倍率")] public float hitMp; // 击中倍率
        [Tooltip("爆头倍率")] public float headshotMp; // 爆头倍率
        [Tooltip("射击间隔")] public float shootCd; // 射击间隔
        [Tooltip("换弹时间")] public float reloadCd; // 换弹时间
        [Tooltip("换弹声音")] public AudioClip reloadAudioClip;
        [Tooltip("射击声音")] public AudioClip shootAudioClip;
        [Tooltip("切换声音")] public AudioClip getWeaponAudioClip;
    }
}