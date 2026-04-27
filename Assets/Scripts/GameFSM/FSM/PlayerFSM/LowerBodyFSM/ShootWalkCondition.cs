using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootWalkCondition : AbstractCondition<PlayerManager>
    {
        public ShootWalkCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "ShootWalk";
            return MyManager.IsShoot && MyManager.IsGround &&
                   MyManager.InputMove != Vector2.zero &&
                   !MyManager.IsSprint && !MyManager.IsSliding &&
                   !MyManager.IsReload;
        }
    }
}