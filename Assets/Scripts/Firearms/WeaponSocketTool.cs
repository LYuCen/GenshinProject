using IUtility;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Firearms
{
    public class WeaponSocketTool : MonoBehaviour
    {
        public WeaponClipAsset clipAsset;
        public Transform weaponPivot; 

        [Header("右手偏移")]
        public Vector3 rOffsetPos;
        public Vector3 rOffsetRot;

        [Header("左手偏移")]
        public Vector3 lOffsetPos;
        public Vector3 lOffsetRot;

        [Header("手动权重 (如果没有曲线则用这个)")]
        [Range(0, 1)] public float handBlend = 1f;
        
        public bool isEditing = false; 

        private AnimationClipPlayable currentPlayable;

        public void SetActiveAnimation(AnimationClipPlayable p, string clipName) {
            currentPlayable = p;
        }

        void LateUpdate()
        {
            if (isEditing || clipAsset == null || !currentPlayable.IsValid()) return;

            // 1. 获取时长和绝对时间 (单位：秒)
            float duration = currentPlayable.GetAnimationClip().length;
            float t = (float)currentPlayable.GetTime();
            float loopTime = t % duration;

            // --- 核心修正：计算归一化时间 (0 到 1) ---
            float normalizedTime = loopTime / duration;

            // 2. 你的手动曲线 (权重)：使用归一化时间 (0 - 1) 采样
            if (clipAsset.handWeight != null && clipAsset.handWeight.length > 0)
            {
                // 现在它会正确映射你画的 0-1 的横坐标了！
                handBlend = clipAsset.handWeight.Evaluate(normalizedTime);
            }

            // 3. 烘焙曲线 (位移旋转)：必须继续使用绝对时间 (loopTime) 采样
            Vector3 rBP = CompressionUtility.GetPos(clipAsset.rPosX, clipAsset.rPosY, clipAsset.rPosZ, loopTime);
            Quaternion rBR = CompressionUtility.GetRot(clipAsset.rRotX, clipAsset.rRotY, clipAsset.rRotZ, loopTime);
            Vector3 lBP = CompressionUtility.GetPos(clipAsset.lPosX, clipAsset.lPosY, clipAsset.lPosZ, loopTime);
            Quaternion lBR = CompressionUtility.GetRot(clipAsset.lRotX, clipAsset.lRotY, clipAsset.lRotZ, loopTime);

            // 4. 应用你的偏移
            Vector3 finalRP = rBP + (rBR * rOffsetPos);
            Quaternion finalRR = rBR * Quaternion.Euler(rOffsetRot);
            Vector3 finalLP = lBP + (lBR * lOffsetPos);
            Quaternion finalLR = lBR * Quaternion.Euler(lOffsetRot);

            // 5. 应用混合 (此时的 handBlend 已经是正确的 0-1 进度了)
            weaponPivot.localPosition = Vector3.Lerp(finalLP, finalRP, handBlend);
            weaponPivot.localRotation = Quaternion.Slerp(finalLR, finalRR, handBlend);
        }
    }
}