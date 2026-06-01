using System;
using Connect;
using Connect.Converter;
using Connect.Data;
using Connect.Protocol;
using Firearms.Bullet;
using mController;
using mModel;
using mSystem.BulletSystem;
using mWeapon.Component;
using mWeapon.WeaponFeature;
using UnityEngine;

namespace mWeapon.Presenter
{
    public class WeaponFirePresenter
    {
        private WeaponPro _weapon;
        private Transform _muzzle;
        private CameraController _cameraController;
        
        public WeaponFirePresenter(WeaponPro weapon, Transform muzzle, CameraController cameraController)
        {
            _weapon = weapon;
            _muzzle = muzzle;
            _cameraController = cameraController;
        }

        private void OnFire()
        {
            var fireFeature = _weapon.RequireFeature<FireFeature>();
            var damageProfile = _weapon.RequireComponent<DamageProfileComponent>();
            var mag = _weapon.RequireComponent<MagazineComponent>();
            
            GameObject obj = BulletGameObjectPool.Instance.GetObj();
            obj.transform.position = _muzzle.position;
            obj.transform.forward = (_cameraController.hitEnd - _muzzle.position).normalized;

            Bullet bullet = new Bullet()
            {
                Transform = obj.transform,
                shooterId = NetworkService.Bridge.GetMyId(),
                baseHit = fireFeature.Data.damage * damageProfile.BodyMultiplier,
                headHit = fireFeature.Data.damage * damageProfile.HeadMultiplier,
                element = (int)mag.ElementEnum,
                elementMeasure = fireFeature.Data.measure,
                moveNormalize = (_cameraController.hitEnd - _muzzle.position).normalized,
                speed = fireFeature.Data.muzzleVelocity,
                MoveTime = 3f
            };
            
            // 设置粒子和枪焰
            var ps = bullet.Transform.GetChild(0).GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = ElementModel.Instance.colors[bullet.element];
            
            // 加入子弹系统
            BulletSystem.Instance.EnQueue(bullet);
            
            // 网络
            if (NetworkService.Bridge != null && NetworkService.Bridge.GetMyId() != -1)
            {
                ShootData data = new ShootData
                {
                    shooterId = NetworkService.Bridge.GetMyId(),
                    position = _muzzle.position,
                    forward = (_cameraController.hitEnd - _muzzle.position).normalized,
                    speed = fireFeature.Data.muzzleVelocity,    
                    baseHit = fireFeature.Data.damage * damageProfile.BodyMultiplier,
                    headHit = fireFeature.Data.damage * damageProfile.HeadMultiplier,
                    element = (int)mag.ElementEnum,
                    elementMeasure = fireFeature.Data.measure,
                    moveTime = 3f,
                    serverTick = DateTime.UtcNow.Ticks,
                };
                
                NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_PlayerShoot, ShootDataConverter.ToDto(data)));
            }
        }
        
        public void OnEnter()
        {
            _weapon.GetFeature<FireFeature>().OnFire += OnFire;
        }

        public void OnExit()
        {
            _weapon.GetFeature<FireFeature>().OnFire -= OnFire;
        }
    }
}