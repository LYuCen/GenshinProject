using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class WalkCondition : AbstractCondition<PlayerManager>
    {
        public WalkCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            return MyManager.isGround && !MyManager.isJump &&
                   MyManager.inputMove != Vector2.zero && !MyManager.isShoot && !MyManager.isSprint;
        }
    }
}