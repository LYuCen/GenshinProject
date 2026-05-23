using System;
using mWeapon.Component;
using mWeapon.FeatureData;
using mWeapon.Task;

namespace mWeapon.WeaponFeature
{
    public class ReloadFeature : IWeaponFeature
    {
        private WeaponPro _weapon;

        private ReloadFeatureData _data;
        public Action OnReload;

        public ReloadFeature(ReloadFeatureData data)
        {
            _data = data;
        }

        public void Initialize(WeaponPro weapon)
        {
            _weapon = weapon;
            FireFeature fire = weapon.RequireFeature<FireFeature>();
            fire.OnEmptyMag += TryReload;
        }

        public void HandleAction(WeaponAction action)
        {
            if(!action.Reload.IsActive()) return;
            
            if(action.Reload.Down) TryReload();
        }

        public void Tick()
        {
        }

        private void TryReload()
        {
            WeaponRuntime runtime = _weapon.InternalRuntime;

            if(runtime.HasState(WeaponStateTags.Reloading)) return;
            if (_weapon.GetComponent<MagazineComponent>().IsFull) return;
            
            ReloadTask task = new ReloadTask(
                    runtime,
                    _data.reloadDuration,
                    _weapon
                );
            
            _weapon.TaskRunner.Run(task);
            OnReload?.Invoke();
        }
    }
}