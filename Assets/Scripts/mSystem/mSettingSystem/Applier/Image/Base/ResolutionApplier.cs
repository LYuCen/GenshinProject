using mModel.SettingModel;
using UnityEngine;
using Screen = UnityEngine.Screen;

namespace mSystem.mSettingSystem.Applier.Image.Base
{
    /// <summary>
    /// 分辨率控制
    /// </summary>
    public class ResolutionApplier : ISettingApplier
    {
        public string Id => "base_resolution";
        public void Apply(int value, SettingSaveData data)
        {
            var res = data.items.Find(c => c.id == "base_display");
            if (res != null && res.value != 2) return;
            switch (value)
            {
                case 0:
                    Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                    break;
                case 1:
                    Screen.SetResolution(2560, 1440, FullScreenMode.Windowed);
                    break;
            }
        }
    }
}