using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class IdleCondition : AbstractCondition<PlayerManager>
    {
        public IdleCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
            
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Idle";
            return MyManager.IsGround && !MyManager.IsJump &&
                   MyManager.InputMove == Vector2.zero &&
                   !MyManager.IsShoot && !MyManager.IsSprint
                   && !MyManager.IsSliding;
        }
    }
}