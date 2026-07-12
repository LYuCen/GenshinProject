using mModel.SettingModel;

namespace mSystem.mSettingSystem
{
    public interface ISettingApplier
    {
        string Id { get; }
        void Apply(int value, SettingSaveData data);
    }
}