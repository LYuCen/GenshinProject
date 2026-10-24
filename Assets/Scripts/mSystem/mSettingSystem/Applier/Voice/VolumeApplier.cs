using mModel.SettingModel;
using QFramework;

namespace mSystem.mSettingSystem.Applier.Voice
{
    public class VolumeApplier : ISettingApplier, IController
    {
        public string Id => "base_volume";
        public void Apply(int value, SettingSaveData data)
        {
            var volume = value / 100f;
            AudioKit.Settings.MusicVolume.Value = volume;
            AudioKit.Settings.VoiceVolume.Value = volume;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}