using Firearms;
using mSystem.BuffSystem.ElementBuff;
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
        private BulletInfo[] _bullets;
        private AbstractGun _primaryWeapon;
        private AbstractGun _secondaryWeapon;
        protected override void OnInit()
        {
            _bullets = new BulletInfo[3];
        }

        public void SetPrimaryWeapon(AbstractGun gun)
        {
            _primaryWeapon = gun;
        }

        public void SetSecondaryWeapon(AbstractGun gun)
        {
            _secondaryWeapon = gun;
        }

        public AbstractGun GetPrimaryWeapon() => _primaryWeapon;
        public AbstractGun GetSecondaryWeapon() => _secondaryWeapon;
        public BulletInfo GetBullet(int i) => _bullets[i];

        public BulletInfo GetBullet(ElementEnum e)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_bullets[i].Data == e) return _bullets[i];
            }

            return new BulletInfo();
        }

        public void SetBullet(int i, ElementEnum data, int val)
        {
            _bullets[i] = new BulletInfo { Data = data, Val = val };
        }

        public void ReturnBullet(BulletInfo info)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_bullets[i].Data == info.Data) _bullets[i].Val += info.Val;
            }
        }

        public void DeductBullet(BulletInfo info)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_bullets[i].Data == info.Data) _bullets[i].Val -= info.Val;
            }
        }
    }
}