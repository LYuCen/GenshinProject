using mModel.SettingModel;
using UnityEngine;

namespace mSystem.mSettingSystem.Applier.Image.Base
{
    /// <summary>
    /// 显示控制
    /// </summary>
    public class DisplayApplier : ISettingApplier
    {
        public string Id => "base_display";
        public void Apply(int value, SettingSaveData data)
        {
            var res = Screen.currentResolution;
            switch (value)
            {
                case 0:
                    Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
                    break;

                case 1:
                    Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
                    break;

                case 2:
                    
                    break;
            }
        }
    }
}