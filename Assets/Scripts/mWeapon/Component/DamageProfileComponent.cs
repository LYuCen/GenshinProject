using mWeapon.ComponentData;

namespace mWeapon.Component
{
    public class DamageProfileComponent
    {
        public float HeadMultiplier;
        public float BodyMultiplier;
        public float LimbMultiplier;
        
        public DamageProfileComponent(DamageProfileComponentData data)
        {
            HeadMultiplier = data.headMultiplier;
            BodyMultiplier = data.bodyMultiplier;
            LimbMultiplier = data.limbMultiplier;
        }
    }
}