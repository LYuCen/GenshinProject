using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class RunCondition : AbstractCondition<PlayerManager>
    {
        public RunCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Run";
            return MyManager.isGround && !MyManager.isJump &&
                   MyManager.inputMove != Vector2.zero
                   && !MyManager.isShoot && MyManager.isSprint
                   && !MyManager.isSliding;
        }
    }
}