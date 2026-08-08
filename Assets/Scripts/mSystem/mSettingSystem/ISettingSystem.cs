using System.Collections.Generic;
using mModel.SettingModel;
using mSystem.mSettingSystem.Applier.Image.Base;
using mSystem.mSettingSystem.Applier.Voice;
using QFramework;

namespace mSystem.mSettingSystem
{
    public interface ISettingSystem : ISystem
    {
        public void RegisterApplier(ISettingApplier applier);
        public void ApplySingle(string id, int value, SettingSaveData data = null);
        public void Apply();
    }

    public class SettingSystem : AbstractSystem, ISettingSystem
    {
        private readonly Dictionary<string, ISettingApplier> _appliers = new();

        protected override void OnInit()
        {
            RegisterApplier(new DisplayApplier());
            RegisterApplier(new ResolutionApplier());
            RegisterApplier(new BrightnessApplier());
            RegisterApplier(new CameraAAApplier());
            RegisterApplier(new VolumeApplier());
            RegisterApplier(new SliderApplier());
        }

        public void RegisterApplier(ISettingApplier applier)
        {
            _appliers[applier.Id] = applier;
        }

        public void ApplySingle(string id, int value, SettingSaveData data = null)
        {
            _appliers[id].Apply(value, data ?? this.GetModel<ISettingModel>().SettingSaveData);
        }

        public void Apply()
        {
            var saveData = this.GetModel<ISettingModel>().SettingSaveData;
            foreach (var item in saveData.items)
            {
                if (_appliers.TryGetValue(item.id, out var applier))
                {
                    applier.Apply(item.value, saveData);
                }
            }
        }
    }
}