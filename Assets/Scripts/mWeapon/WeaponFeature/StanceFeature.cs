using mEvent;
using mSystem.InputSystem;
using mWeapon.FeatureData;
using QFramework;
using UnityEngine;

namespace mWeapon.WeaponFeature
{
    public class StanceFeature : IWeaponFeature, ICanSendEvent
    {
        private WeaponPro _weapon;
        private readonly StanceFeatureData _data;

        private WeaponStance CurrentStance => _weapon.Runtime.Stance;
        private float _holdTimer;
        private bool _enteredShoulder;

        public StanceFeature(StanceFeatureData data)
        {
            _data = data;
        }

        public void Initialize(WeaponPro weapon)
        {
            _weapon = weapon;
            ReloadFeature r = weapon.RequireFeature<ReloadFeature>();
            r.OnReload += ExitShoulder;
        }

        public void HandleAction(WeaponAction action)
        {
            if (!action.Secondary.IsActive()) return;
            if (_weapon.Runtime.HasState(WeaponStateTags.Reloading)) return;
            HandleSecondary(action.Secondary);
        }

        public void Tick()
        {
            
        }

        private void HandleSecondary(ButtonState secondary)
        {
            // 按下
            if (secondary.Down)
            {
                _holdTimer = 0;

                // ADS -> 退出
                if (CurrentStance == WeaponStance.Ads)
                {
                    SetStance(WeaponStance.Normal);
                    return;
                }
                EnterShoulder();
            }

            // 按住
            if (secondary.Press)
            {
                _holdTimer += Time.deltaTime;
            }

            // 松开
            if (secondary.Up)
            {
                // 已经退出 ADS
                if (CurrentStance == WeaponStance.Normal)
                {
                    return;
                }

                // 快点 -> ADS
                if (_holdTimer < _data.shoulderThreshold)
                {
                    ToggleAds();
                }
                // 长按 -> 退出 Shoulder
                else
                {
                    ExitShoulder();
                }
            }
        }

        /// <summary>
        /// 切换 ADS
        /// </summary>
        private void ToggleAds()
        {
            if (CurrentStance == WeaponStance.Ads)
            {
                SetStance(WeaponStance.Normal);
            }
            else
            {
                SetStance(WeaponStance.Ads);
            }
        }

        /// <summary>
        /// 进入肩射
        /// </summary>
        private void EnterShoulder()
        {
            SetStance(WeaponStance.Shoulder);
        }

        /// <summary>
        /// 退出肩射
        /// </summary>
        private void ExitShoulder()
        {
            SetStance(WeaponStance.Normal);
        }

        private void SetStance(WeaponStance stance)
        {
            _weapon.InternalRuntime.SetStance(stance);
            this.SendEvent<FrontSightEvent>();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}