using mController;
using mEvent;
using mSystem.SoundSystem;
using mWeapon.Component;
using mWeapon.WeaponFeature;
using QFramework;
using UnityEngine;

namespace mWeapon.Presenter
{
    /// <summary>
    /// 武器实例
    /// </summary>
    public class WeaponPresenter : MonoBehaviour, ICanSendEvent, IController
    {
        public Transform magazine;
        public Transform hand;
        public Transform muzzle;
        public Transform gunSight;
        private WeaponPro _weapon;

        private WeaponAudioPresenter _audioPresenter;
        private WeaponFirePresenter _firePresenter;
        private WeaponMuzzlePresenter _muzzlePresenter;

        private bool _isInit;
        public void Init(WeaponPro weapon, CameraController  cameraController)
        {
            if (_isInit) return;
            _isInit = true;
            _weapon = weapon;
            _audioPresenter = new WeaponAudioPresenter(weapon, this.GetSystem<IAudioSystem>());
            _firePresenter = new WeaponFirePresenter(weapon, muzzle, cameraController);
            _muzzlePresenter = new WeaponMuzzlePresenter(weapon, muzzle);

            var boltFeature = weapon.GetFeature<BoltFeature>();
        }
        
        public void OnEnter()
        {
            ResetTrans();
            this.SendEvent<WeaponDataUpdateEvent>();
                
            _audioPresenter?.OnEnter();

            _firePresenter?.OnEnter();
            
            _muzzlePresenter?.OnEnter();
        }

        public void OnExit()
        {
            if (!this) return;
            gameObject.SetActive(false);

            _audioPresenter?.OnExit();

            _firePresenter?.OnExit();

            _muzzlePresenter?.OnExit();
        }

        public void ResetTrans()
        {
            if (_weapon == null) return;

            transform.localPosition = _weapon.RequireComponent<TransformComponent>().Pos;
            Vector3 rot = _weapon.RequireComponent<TransformComponent>().Rot;
            transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        }
        
        private void OnDestroy()
        {
            _firePresenter?.OnExit();
            _muzzlePresenter?.OnExit();
            _audioPresenter?.OnExit();
        }


        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}