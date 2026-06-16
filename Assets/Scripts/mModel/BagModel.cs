using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using Firearms;
using mSystem.BuffSystem.ElementBuff;
using mWeapon;
using QFramework;

namespace mModel
{
    public struct BulletInfo
    {
        public ElementEnum Data;
        public int Val;

        public BulletInfo(ElementEnum data, int val)
        {
            Data = data;
            Val = val;
        }
    }
    
    public class BagModel : AbstractModel
    {
        private List<BulletInfo> _bullets;
        private AbstractGun _primaryWeapon;
        private AbstractGun _secondaryWeapon;
        private WeaponPro _fnIws;
        private WeaponPro _spr2;
        public WeaponPro Weapon { get; private set; }
        private int _index;
        protected override void OnInit()
        {
            _bullets = new List<BulletInfo>();
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/ScriptableObject/Weapon/FnIws/FnIws.asset",
                false);
            _fnIws = new WeaponPro(ir.GetAsset<WeaponData>());
            var ir2 = ResourceManager.instance.Load("Assets/AssetBundle/ScriptableObject/Weapon/Spr2/Spr2.asset",
                false);
            _spr2 = new WeaponPro(ir2.GetAsset<WeaponData>());
            Weapon = _fnIws;
        }

        public void SetPrimaryWeapon()
        {
            Weapon = _fnIws;
        }

        public void SetSecondaryWeapon()
        {
            Weapon = _spr2;
        }

        public AbstractGun GetPrimaryWeapon() => _primaryWeapon;
        public AbstractGun GetSecondaryWeapon() => _secondaryWeapon;
        public BulletInfo GetBullet(int i) => _bullets[i];

        public BulletInfo GetBullet(ElementEnum e)
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i].Data == e) return _bullets[i];
            }

            return new BulletInfo();
        }

        public void SetBulletIndex(int val)
        {
            _index = val;
        }

        public ElementEnum GetBullet()
        {
            return _bullets[_index].Data;
        }

        public void SetBullet(int i, ElementEnum data, int val)
        {
            while (_bullets.Count <= i) _bullets.Add(new BulletInfo());       
            var bullet = _bullets[i];
            bullet.Data = data;
            bullet.Val = val;
            _bullets[i] = bullet;
        }


        public void ReturnBullet(BulletInfo info)
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i].Data == info.Data)
                {
                    var bullet = _bullets[i];
                    bullet.Val += info.Val;      
                    _bullets[i] = bullet;        
                }
            }
        }

        public void DeductBullet(BulletInfo info)
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i].Data == info.Data)
                {
                    var bullet = _bullets[i];    
                    bullet.Val -= info.Val;      
                    _bullets[i] = bullet;        
                }
            }

        }

        public int BulletCount => _bullets.Count;
    }
}