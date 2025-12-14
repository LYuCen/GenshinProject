using mSystem.BuffSystem.ElementBuff;

namespace Firearms.Magazine
{
    public class BaseMagazine : AbstractMagazine
    {
        public BaseMagazine(int bulletNumber,int bulletCapacity,int baseHurt,ElementEnum element)
        {
            BulletNumber = bulletNumber;
            BulletCapacity = bulletCapacity;
            BaseHurt = baseHurt;
            Element = element;
        }
    }
}