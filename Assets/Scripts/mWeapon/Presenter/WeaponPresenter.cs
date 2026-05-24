using mController;
using mWeapon.Component;
using UnityEngine;

namespace mWeapon.Presenter
{
    /// <summary>
    /// 武器实例
    /// </summary>
    public class WeaponPresenter : MonoBehaviour
    {
        public Transform magazine;
        public Transform hand;
        public Transform muzzle;
        public Transform gunSight;
        private WeaponPro _weapon;
        private CameraController _cameraController;

        private WeaponAudioPresenter _audioPresenter;
        private WeaponFirePresenter _firePresenter;
        private WeaponMuzzlePresenter _muzzlePresenter;

        private bool _isInit;
        public void Init(WeaponPro weapon, CameraController  cameraController)
        {
            if (_isInit) return;
            _isInit = true;
            _weapon = weapon;
            _cameraController = cameraController;
            _audioPresenter = new WeaponAudioPresenter(weapon);
            _firePresenter = new WeaponFirePresenter(weapon, muzzle, cameraController);
            _muzzlePresenter = new WeaponMuzzlePresenter(weapon, muzzle);
        }
        
        public void OnEnter()
        {
            transform.localPosition = _weapon.RequireComponent<TransformComponent>().Pos;
            Vector3 rot = _weapon.RequireComponent<TransformComponent>().Rot;
            transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
                
            _audioPresenter?.OnEnter();

            _firePresenter?.OnEnter();
            
            _muzzlePresenter?.OnEnter();
        }

        public void OnExit()
        {
            _audioPresenter?.OnExit();

            _firePresenter?.OnExit();
            
            _muzzlePresenter?.OnExit();
        }
    }
}