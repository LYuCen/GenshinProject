using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class WalkCondition : AbstractCondition<PlayerManager>
    {
        public WalkCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            return MyManager.IsGround && !MyManager.IsJump &&
                   MyManager.InputMove != Vector2.zero &&
                   !MyManager.IsShoot && !MyManager.IsSprint
                   && !MyManager.IsSliding;
        }
    }
}