using mWeapon.FeatureData;
using mWeapon.WeaponFeature;

namespace mWeapon
{
    public static class WeaponFeatureFactory
    {
        public static IWeaponFeature Create(
            WeaponFeatureData data)
        {
            switch (data)
            {
                case FireFeatureData fire:
                    return new FireFeature(fire);

                case BoltFeatureData bolt:
                    return new BoltFeature(bolt);
                
                case ReloadFeatureData reload:
                    return new ReloadFeature(reload);
                
                case StanceFeatureData stance:
                    return new StanceFeature(stance);
            }

            return null;
        }
    }
}