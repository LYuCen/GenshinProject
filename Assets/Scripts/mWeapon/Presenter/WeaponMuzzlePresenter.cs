using Firearms;
using Firearms.Muzzle;
using mModel;
using mWeapon.WeaponFeature;
using UnityEngine;

namespace mWeapon.Presenter
{
    public class WeaponMuzzlePresenter
    {
        private WeaponPro _weapon;
        private Transform _muzzle;
        
        public WeaponMuzzlePresenter(WeaponPro weapon, Transform muzzle)
        {
            _weapon = weapon;
            _muzzle = muzzle;
        }

        private void OnFire()
        {
            GameObject muzzleObj = FireMuzzleObjectPool.Instance.GetObj();
            muzzleObj.transform.SetParent(_muzzle);
            muzzleObj.gameObject.SetActive(true);
            muzzleObj.transform.localPosition = Vector3.zero;
            muzzleObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            Muzzle muzzle = muzzleObj.GetComponent<Muzzle>();
            muzzle.UpdateColor(ElementModel.Instance.colors[(int)_weapon.Runtime.Component.ElementEnum]);
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