using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class CharacterEffectManager : MonoBehaviour
    {
        public Material effectMaterial;

        [Header("动效时间")]
        public float fadeInTime = 0.1f;    // 再次受击时回弹的速度（越快越有打击感）
        public float fadeOutTime = 1.2f;   // 消失的速度
    
        [Header("亮度设置")]
        [ColorUsage(true, true)] public Color baseColor = Color.cyan;
        public float maxIntensity = 5.0f; 

        private Renderer[] allRenderers;
        private Material instanceMaterial;
        private Coroutine currentAnim;

        // 记录当前的实时状态
        private float currentIntensity = 0f;
        private float currentAlpha = 0f;

        void Awake()
        {
            if (effectMaterial != null)
            {
                instanceMaterial = new Material(effectMaterial);
                // 初始设为全透明
                ResetMaterialAlpha(0);
            }
        }

        [ContextMenu("Play Hit Effect")]
        public void PlayAuraAnimation(Color color)
        {
            if (!instanceMaterial) return;

            // 更新基准色
            baseColor = color; 

            if (currentIntensity <= 0.01f) UpdateEffectState(true);
            if (currentAnim != null) StopCoroutine(currentAnim);
            currentAnim = StartCoroutine(AuraAnimationRoutine());
        }

        private IEnumerator AuraAnimationRoutine()
        {
            // --- 阶段一：从当前状态快速回弹到最高亮度 ---
            float startI = currentIntensity; // 读取当前亮度（可能在 2.0，也可能在 0.5）
            float startA = currentAlpha;
        
            float elapsed = 0;
            while (elapsed < fadeInTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInTime;
            
                // 从当前值线性插值到最大值
                currentIntensity = Mathf.Lerp(startI, maxIntensity, t);
                currentAlpha = Mathf.Lerp(startA, baseColor.a, t);
            
                ApplyToMaterial(currentIntensity, currentAlpha);
                yield return null;
            }

            // --- 阶段二：从最高点慢慢衰减到 0 ---
            elapsed = 0;
            while (elapsed < fadeOutTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeOutTime;

                currentIntensity = Mathf.Lerp(maxIntensity, 0, t);
                currentAlpha = Mathf.Lerp(baseColor.a, 0, t);

                ApplyToMaterial(currentIntensity, currentAlpha);
                yield return null;
            }

            // --- 阶段三：彻底消失，移除槽位 ---
            currentIntensity = 0;
            currentAlpha = 0;
            ApplyToMaterial(0, 0);
            UpdateEffectState(false);
            currentAnim = null;
        }

        private void ApplyToMaterial(float intensity, float alpha)
        {
            Color c = baseColor;
            c.a = alpha;
            instanceMaterial.SetColor("_AuraColor", c);
            instanceMaterial.SetFloat("_AuraIntensity", intensity);
        }

        private void ResetMaterialAlpha(float val)
        {
            currentIntensity = val;
            currentAlpha = val;
            ApplyToMaterial(val, val);
        }

        public void UpdateEffectState(bool enable)
        {
            allRenderers = GetComponentsInChildren<Renderer>();
            foreach (var ren in allRenderers)
            {
                if (ren == null) continue;
                List<Material> mats = new List<Material>(ren.sharedMaterials);
                if (enable)
                {
                    if (!mats.Contains(instanceMaterial)) mats.Add(instanceMaterial);
                }
                else
                {
                    mats.Remove(instanceMaterial);
                }
                ren.sharedMaterials = mats.ToArray();
            }
        }

        // 武器切换时调用
        public void RefreshWeapon()
        {
            if (currentIntensity > 0) UpdateEffectState(true);
        }
    }
}