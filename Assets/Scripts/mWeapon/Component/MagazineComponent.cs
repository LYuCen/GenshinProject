using mSystem.BuffSystem.ElementBuff;
using mWeapon.ComponentData;

namespace mWeapon.Component
{
    public class MagazineComponent
    {
        public int BulletVal;

        public readonly int MagSize;

        public ElementEnum ElementEnum;

        public MagazineComponent(MagazineComponentData data)
        {
            MagSize = data.magazineCapacity;

            ElementEnum = data.magazineElement;

            BulletVal = MagSize;
        }

        public bool IsEmpty => BulletVal <= 0;

        public bool IsFull => BulletVal >= MagSize;
    }
}