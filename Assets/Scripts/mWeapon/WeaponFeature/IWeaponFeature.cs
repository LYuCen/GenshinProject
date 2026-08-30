namespace mWeapon.WeaponFeature
{
    public interface IWeaponFeature
    {
        void Initialize(WeaponPro weapon);

        void HandleAction(WeaponAction action);

        void Tick(float dt);
        
        void OnEnter();

        void OnExit();
    }
}