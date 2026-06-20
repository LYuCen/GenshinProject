using mEnum.Query;
using UnityEngine;

namespace mInterface
{
    public interface ICombatQueryProvider
    {
        WeaponQuery GetQuery();
        Transform GetWeapon();
        void ResetGun();
    }
}