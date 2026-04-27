using UnityEngine;
using UnityEngine.Serialization;

namespace Firearms
{
    // 改为 ScriptableObject 并在菜单中增加创建选项
    [CreateAssetMenu(fileName = "WeaponClipAsset", menuName = "Weapon/Clip Asset")]
    public class WeaponClipAsset : ScriptableObject 
    {
        public string clipName;

        [Header("位移旋转曲线 (使用绝对时间: 秒)")] public AnimationCurve rPosX;
        public AnimationCurve rPosY, rPosZ;
        public AnimationCurve rRotX, rRotY, rRotZ;
        public AnimationCurve lPosX, lPosY, lPosZ;
        public AnimationCurve lRotX, lRotY, lRotZ;

        [Header("权重控制曲线 (使用归一化时间: 0-1)")]
        public AnimationCurve handWeight = AnimationCurve.Constant(0, 1, 1);
        public AnimationCurve followWeight = AnimationCurve.Constant(0, 1, 1);

        [Header("该动画专属偏移")]
        public Vector3 rOffset = Vector3.zero;
        public Vector3 rRot = Vector3.zero;
        public Vector3 lOffset = Vector3.zero;
        public Vector3 lRot = Vector3.zero;
    }
}