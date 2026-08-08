using mModel.SettingModel;
using QFramework;
using UnityEngine;

namespace mSystem.mSettingSystem.Applier.Image.Base
{
    /// <summary>
    /// 显示控制
    /// </summary>
    public class DisplayApplier : ISettingApplier, IController
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
                    var val = data.items.Find(c => c.id == "base_resolution").value;
                    this.GetSystem<ISettingSystem>().ApplySingle("base_resolution", val);
                    break;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}