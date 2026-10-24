using mSystem.SoundSystem;
using mWeapon.Component;
using mWeapon.WeaponFeature;

namespace mWeapon.Presenter
{
    public class WeaponAudioPresenter
    {
        private WeaponPro _weapon;
        private IAudioSystem _audioSystem;
        
        public WeaponAudioPresenter(WeaponPro weapon, IAudioSystem audioSystem)
        {
            _weapon = weapon;
            _audioSystem = audioSystem;
        }

        private void OnFire()
        {
            _audioSystem.PlaySound(_weapon.RequireComponent<AudioComponent>().FireAudioClip);
        }
        
        public void OnGet()
        {
            _audioSystem.PlaySound(_weapon.RequireComponent<AudioComponent>().GetAudioClip);
        }

        private void OnReload()
        {
            _audioSystem.PlaySound(_weapon.RequireComponent<AudioComponent>().ReloadAudioClip);
        }

        private void OnBolt()
        {
            _audioSystem.PlaySound(_weapon.RequireComponent<AudioComponent>().BoltAudioClip);
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