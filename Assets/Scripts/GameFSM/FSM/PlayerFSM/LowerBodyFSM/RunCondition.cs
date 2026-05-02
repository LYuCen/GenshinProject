using GameFSM.LevelFSM.PlayerFSM;
using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class RunCondition : AbstractCondition<PlayerManager>
    {
        public RunCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Run";
            return MyManager.IsGround && !MyManager.IsJump &&
                   MyManager.InputMove != Vector2.zero
                   && !MyManager.IsShoot && MyManager.IsSprint
                   && !MyManager.IsSliding;
        }
    }
}