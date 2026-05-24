using mWeapon.Component;
using mWeapon.ComponentData;

namespace mWeapon
{
    public static class WeaponComponentFactory
    {
        public static object Create(WeaponComponentData data)
        {
            switch (data)
            {
                case MagazineComponentData mag:
                    return new MagazineComponent(mag);
                case DamageProfileComponentData dam:
                    return new DamageProfileComponent(dam);
                case AudioComponentData audio:
                    return new AudioComponent(audio);
                case TransformComponentData trans:
                    return new TransformComponent(trans);
            }
            return null;
        }
    }
}