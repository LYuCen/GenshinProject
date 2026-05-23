using System;
using Connect;
using Connect.Data;
using mController;
using mWeapon.Component;
using mWeapon.WeaponFeature;
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

        private bool _isInit;
        public void Init(WeaponPro weapon, CameraController  cameraController)
        {
            if (_isInit) return;
            _isInit = true;
            _weapon = weapon;
            _cameraController = cameraController;
            _audioPresenter = new WeaponAudioPresenter(weapon);
            _weapon.RequireFeature<FireFeature>().OnFire += OnFire;
        }

        private void OnFire()
        {
            var fireFeature = _weapon.RequireFeature<FireFeature>();
            var damageProfile = _weapon.RequireComponent<DamageProfileComponent>();
            var mag = _weapon.RequireComponent<MagazineComponent>();
            ShootData data = new ShootData
            {
                shooterId = NetworkService.Bridge.GetMyId(),
                position = muzzle.position,
                forward = (_cameraController.rayEnd - muzzle.position).normalized,
                speed = fireFeature.Data.muzzleVelocity,
                baseHit = fireFeature.Data.damage * damageProfile.BodyMultiplier,
                headHit = fireFeature.Data.damage * damageProfile.HeadMultiplier,
                element = (int)mag.ElementEnum,
                elementMeasure = 1f,
                moveTime = 3f,
                serverTick = DateTime.UtcNow.Ticks,
            };
        }
    }
}