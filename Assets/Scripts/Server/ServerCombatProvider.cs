using Connect;
using Connect.Data;
using mEnum.Flags;
using mEnum.Query;
using mInterface;
using UnityEngine;

namespace Server
{
    public class ServerCombatProvider : ICombatQueryProvider
    {
        private PlayerStateData _state;
        private WeaponQuery _query;
        private int _weaponBoolState;

        public ServerCombatProvider(PlayerStateData state)
        {
            _state = state;
        }

        public void SetWeaponBoolState(int weaponBoolState)
        {
            _weaponBoolState = weaponBoolState;
        }

        public WeaponQuery GetQuery()
        {
            _query.SetState(_weaponBoolState);
            return _query;
        }

        public Transform GetWeapon() => null;
        public void ResetGun() { }
    }
}
