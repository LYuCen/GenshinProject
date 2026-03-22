using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootWalkCondition : AbstractCondition<PlayerManager>
    {
        public ShootWalkCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "ShootWalk";
            return MyManager.isShoot && MyManager.isGround && MyManager.inputMove != Vector2.zero && !MyManager.isSprint;
        }
    }
}