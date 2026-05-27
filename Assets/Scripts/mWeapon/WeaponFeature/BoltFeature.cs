using System;
using mEvent;
using mWeapon.FeatureData;
using QFramework;
using UnityEngine;

namespace mWeapon.WeaponFeature
{
    public class BoltFeature : IWeaponFeature, ICanSendEvent
    {
        
        private WeaponPro _weapon;

        private BoltFeatureData _data;

        private float _timer;
        public Action OnBolt;

        public BoltFeature(BoltFeatureData data)
        {
            _data = data;
        }

        public void Initialize(WeaponPro weapon)
        {
            _weapon = weapon;

            FireFeature fire = weapon.RequireFeature<FireFeature>();
            fire.OnFire += StartBolt;
        }

        public void HandleAction(WeaponAction action)
        {
        }

        public void Tick()
        {
            if(!_weapon.Runtime.HasState(WeaponStateTags.Bolting)) return;

            _timer -= Time.deltaTime;

            if(_timer <= 0)
            {
                _weapon.InternalRuntime.RemoveState(WeaponStateTags.Bolting);
                Debug.Log("拉栓结束");
            }
        }

        public void OnEnter()
        {
            
        }

        public void OnExit()
        {
            
        }

        private void StartBolt()
        {
            _weapon.InternalRuntime.AddState(WeaponStateTags.Bolting);

            _timer = _data.boltDuration;
            this.SendEvent(new BoltEvent());
            OnBolt?.Invoke();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}