using mModel.SettingModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace mSystem.mSettingSystem.Applier.Image.Base
{
    /// <summary>
    /// 亮度控制
    /// </summary>
    public class BrightnessApplier : ISettingApplier
    {
        public string Id => "base_brightness";

        private Volume _volume;
        private ColorAdjustments _colorAdjustments;
        
        public void Apply(int value, SettingSaveData data)
        {
            if(_volume == null) _volume = Object.FindFirstObjectByType<Volume>();
            
            if (_volume == null || _volume.profile == null)
            {
                Debug.LogError("Volume not assigned!");
                return;
            }

            if (!_volume.profile.TryGet(out _colorAdjustments))
            {
                Debug.LogError("ColorAdjustments not found in Volume Profile!");
            }

            float normalized = Mathf.Clamp(value, 0, 200) / 100f;

            // 映射到 postExposure
            float exposure = Mathf.Lerp(-1f, 1f, normalized / 2f);

            _colorAdjustments.postExposure.value = exposure;
        }
    }
}