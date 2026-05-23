using mEvent;
using mModel;
using mWeapon.Component;
using QFramework;
using UnityEngine;

namespace mWeapon.Task
{
    public class ReloadTask : IWeaponTask, ICanSendEvent
    {
        private readonly WeaponRuntime _runtime;
        private float _timer;
        private readonly float _reloadTime;
        private WeaponPro _weapon;
        public bool Finished { get; private set; }
        public ReloadTask(WeaponRuntime runtime, float reloadTime, WeaponPro weapon)
        {
            _runtime = runtime;
            _reloadTime = reloadTime;
            _weapon = weapon;
        }
        public void Start()
        {
            _runtime.AddState(WeaponStateTags.Reloading);
            _timer = _reloadTime;
            Debug.Log("换弹开始");
        }
        public void Tick()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Finish();
            }
        }
        private void Finish()
        {
            _runtime.RemoveState(WeaponStateTags.Reloading);
            MagazineComponent mag = _weapon.GetComponent<MagazineComponent>();
            BulletInfo info = new BulletInfo(mag.ElementEnum, mag.BulletVal);
            _weapon.BagModel.ReturnBullet(info);
            mag.BulletVal = Mathf.Min(_weapon.BagModel.GetBullet(mag.ElementEnum).Val, mag.MagSize);
            info = new BulletInfo(mag.ElementEnum, mag.BulletVal);
            _weapon.BagModel.DeductBullet(info);
            Debug.Log("换弹结束");
            this.SendEvent<WeaponDataUpdateEvent>();
            Finished = true;
        }
        
        public void Cancel()
        {
            _runtime.RemoveState(WeaponStateTags.Reloading);
            Finished = true;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}