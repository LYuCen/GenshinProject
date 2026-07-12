using mModel.SettingModel;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace mSystem.mSettingSystem.Applier.Image.Base
{
    public class CameraAAApplier : ISettingApplier
    {
        public string Id => "high_AA";
        public void Apply(int value, SettingSaveData data)
        {
            if (Camera.main == null) return;
            var camData = Camera.main.GetComponent<UniversalAdditionalCameraData>();

            switch (value)
            {
                case 0:
                    camData.antialiasing = AntialiasingMode.None;
                    break;
                case 1:
                    camData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                    break;
                case 2:
                    camData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    break;
                case 3:
                    camData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
                    break;
            }
        }
    }
}