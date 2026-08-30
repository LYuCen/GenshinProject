using mModel;
using mSystem.BuffSystem.ElementBuff;

namespace mWeapon
{
    public interface IInventory
    {
        ElementEnum ChosenElement { get; }
        BulletInfo GetBullet(ElementEnum e);
        void ReturnBullet(BulletInfo info);
        void DeductBullet(BulletInfo info);
    }
}