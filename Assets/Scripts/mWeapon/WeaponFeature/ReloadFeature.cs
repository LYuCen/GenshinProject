using System;
using mEvent;
using mWeapon.Component;
using mWeapon.FeatureData;
using mWeapon.Task;
using QFramework;

namespace mWeapon.WeaponFeature
{
    public class ReloadFeature : IWeaponFeature, ICanSendEvent
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

        public void OnEnter()
        {
            
        }

        public void OnExit()
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
            this.SendEvent(new OnReloadEvent());
            OnReload?.Invoke();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}