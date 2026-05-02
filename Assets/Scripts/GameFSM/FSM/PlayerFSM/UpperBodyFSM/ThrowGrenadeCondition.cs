using Firearms.ThrowWeapon;
using GameFSM.LevelFSM.PlayerFSM;

namespace GameFSM.FSM.PlayerFSM.UpperBodyFSM
{
    public class ThrowGrenadeCondition : AbstractCondition<PlayerManager>
    {
        public ThrowGrenadeCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "ThrowGrenade";
            var grenade = MyManager.firearmController.Weapon as Grenade;
            return grenade?.CurrentState != GrenadeState.Idle && MyManager.IsThrow;
        }
    }
}