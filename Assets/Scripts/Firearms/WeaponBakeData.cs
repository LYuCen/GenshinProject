using System.Collections.Generic;
using UnityEngine;

namespace Firearms
{
    [CreateAssetMenu(fileName = "WeaponBakeData", menuName = "Weapon/BakeData")]
    public class WeaponBakeData : ScriptableObject
    {
        [System.Serializable]
        public class ClipData
        {
            public string clipName;
        
            [Header("右手烘焙曲线")]
            public AnimationCurve posX, posY, posZ;
            public AnimationCurve rotX, rotY, rotZ;
    
            [Header("左手烘焙曲线")]
            public AnimationCurve L_posX, L_posY, L_posZ;
            public AnimationCurve L_rotX, L_rotY, L_rotZ;

            [Header("控制权重 (1=全右, 0=全左)")]
            // 默认设为全右手（常量1），这样你没调过曲线的动画依然正常
            public AnimationCurve handWeight = AnimationCurve.Constant(0, 1, 1);

            [Header("跟随权重 (1=跟手, 0=固定在胸部偏移)")]
            // 额外建议增加这个，用于控制换弹拉栓时枪是否“脱手”
            public AnimationCurve followWeight = AnimationCurve.Constant(0, 1, 1);
        }

        public List<ClipData> allClips = new List<ClipData>();
        public ClipData GetClip(string name) => allClips.Find(c => c.clipName == name);
    }
}