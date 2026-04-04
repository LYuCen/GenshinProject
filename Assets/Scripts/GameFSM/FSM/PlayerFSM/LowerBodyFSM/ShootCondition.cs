using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootCondition : AbstractCondition<PlayerManager>
    {
        public ShootCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Shoot";
            return MyManager.IsGround && MyManager.IsShoot &&
                   MyManager.InputMove == Vector2.zero &&
                   !MyManager.IsSprint && !MyManager.IsSliding;
        }
    }
}