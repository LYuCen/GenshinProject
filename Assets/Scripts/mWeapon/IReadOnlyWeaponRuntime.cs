using System;
using mWeapon.Component;

namespace mWeapon
{
    public interface IReadOnlyWeaponRuntime
    {
        MagazineComponent Component { get;}
        WeaponStance Stance { get;}

        bool HasState(WeaponStateTags tag);

        event Action OnAmmoChanged;

        event Action OnStateChanged;
    }
}