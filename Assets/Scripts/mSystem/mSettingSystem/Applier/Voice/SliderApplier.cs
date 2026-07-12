using mModel.SettingModel;
using mSystem.SoundSystem;
using QFramework;

namespace mSystem.mSettingSystem.Applier.Voice
{
    public class SliderApplier : ISettingApplier, IController
    {
        public string Id => "base_slider";
        public void Apply(int value, SettingSaveData data)
        {
            AudioSystem.Instance.SetVolumeValue(value);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}