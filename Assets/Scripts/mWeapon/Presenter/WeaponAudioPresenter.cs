using mSystem.SoundSystem;
using mWeapon.Component;
using mWeapon.WeaponFeature;

namespace mWeapon.Presenter
{
    public class WeaponAudioPresenter
    {
        private WeaponPro _weapon;
        
        public WeaponAudioPresenter(WeaponPro weapon)
        {
            _weapon = weapon;
        }

        private void OnFire()
        {
            AudioSystem.Instance.PlaySound(_weapon.RequireComponent<AudioComponent>().FireAudioClip);
        }
        
        public void OnGet()
        {
            AudioSystem.Instance.PlaySound(_weapon.RequireComponent<AudioComponent>().GetAudioClip);
        }

        private void OnReload()
        {
            AudioSystem.Instance.PlaySound(_weapon.RequireComponent<AudioComponent>().ReloadAudioClip);
        }

        private void OnBolt()
        {
            AudioSystem.Instance.PlaySound(_weapon.RequireComponent<AudioComponent>().BoltAudioClip);
        }
        
        public void OnEnter()
        {
            _weapon.RequireFeature<FireFeature>().OnFire += OnFire;
            _weapon.RequireFeature<ReloadFeature>().OnReload += OnReload;
            if (_weapon.GetFeature<BoltFeature>() != null)
            {
                _weapon.GetFeature<BoltFeature>().OnBolt += OnBolt;
            }
        }

        public void OnExit()
        {
            _weapon.RequireFeature<FireFeature>().OnFire -= OnFire;
            _weapon.RequireFeature<ReloadFeature>().OnReload -= OnReload;
            if (_weapon.GetFeature<BoltFeature>() != null)
            {
                _weapon.GetFeature<BoltFeature>().OnBolt -= OnBolt;
            }
        }
    }
}