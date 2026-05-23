using System;
using mEvent;
using mWeapon.Component;
using mWeapon.FeatureData;
using QFramework;
using UnityEngine;

namespace mWeapon.WeaponFeature
{
    public class FireFeature : IWeaponFeature, ICanSendEvent
    {
        public Action OnFire;
        public Action OnEmptyMag;
        private readonly FireFeatureData _data;
        public FireFeatureData Data => _data;

        private WeaponPro _weapon;
        private float _timer;

        public FireFeature(FireFeatureData data)
        {
            _data = data;
        }

        public void Initialize(WeaponPro weapon)
        {
            _weapon = weapon; 
            
        }

        public void HandleAction(WeaponAction action)
        {
            if (!action.Primary.IsActive()) return;
            
            if(action.Primary.Down) TryFire();
            else if(action.Primary.Press && _data.automatic) TryFire();
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;
        }

        private void TryFire()
        {
            if(_timer > 0) return;
            
            WeaponRuntime runtime = _weapon.InternalRuntime;

            if (runtime.HasState(WeaponStateTags.Reloading)) return;
            if (runtime.HasState(WeaponStateTags.Bolting)) return;
            MagazineComponent mag = _weapon.GetComponent<MagazineComponent>();
            if (mag.IsEmpty)
            {
                OnEmptyMag?.Invoke();
                return;
            }
            _timer = 60f / _data.fireRate;
            mag.BulletVal--;

            Fire();
            this.SendEvent<WeaponDataUpdateEvent>();
            OnFire?.Invoke();
        }

        private void Fire()
        {
            Debug.Log("射击" + _weapon.GetComponent<MagazineComponent>().BulletVal);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}