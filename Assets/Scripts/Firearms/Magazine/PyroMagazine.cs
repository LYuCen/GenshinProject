using mSystem.BuffSystem.ElementBuff;

namespace Firearms.Magazine
{
    public class PyroMagazine : AbstractMagazine
    {
        public PyroMagazine(int bulletNumber,int bulletCapacity,int baseHurt)
        {
            BulletNumber = bulletNumber;
            BulletCapacity = bulletCapacity;
            BaseHurt = baseHurt;
            Element = ElementEnum.Fire;
        }
    }
}