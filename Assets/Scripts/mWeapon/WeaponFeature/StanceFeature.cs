using mSystem.InputSystem;
using mWeapon.FeatureData;

namespace mWeapon.WeaponFeature
{
    public class StanceFeature : IWeaponFeature
    {
        private WeaponPro _weapon;
        private readonly StanceFeatureData _data;

        private WeaponStance CurrentStance => _weapon.Runtime.Stance;
        private float _holdTimer;
        private bool _enteredShoulder;
        private bool _holdAccumulate; // 本 tick 是否累加按住时间

        public StanceFeature(StanceFeatureData data)
        {
            _data = data;
        }

        public void Initialize(WeaponPro weapon)
        {
            _weapon = weapon;
            ReloadFeature r = weapon.RequireFeature<ReloadFeature>();
            BoltFeature b = weapon.GetFeature<BoltFeature>();
            r.OnReload += ExitShoulder;
            if(b != null) b.OnBolt += ExitShoulder;
        }

        public void HandleAction(WeaponAction action)
        {
            _holdAccumulate = false;
            if (!action.Secondary.IsActive()) return;
            if (_weapon.Runtime.HasState(WeaponStateTags.Reloading)) return;
            if (_weapon.Runtime.HasState(WeaponStateTags.Bolting)) return;
            HandleSecondary(action.Secondary);
        }

        public void Tick(float dt)
        {
            if (_holdAccumulate) _holdTimer += dt;
        }

        public void OnEnter()
        {
            
        }

        public void OnExit()
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
            _holdAccumulate = secondary.Press;

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
        }
    }
}