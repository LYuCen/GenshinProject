using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootCondition : AbstractCondition<PlayerManager>
    {
        public ShootCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Shoot";
            return MyManager.isGround && MyManager.isShoot && MyManager.inputMove == Vector2.zero && !MyManager.isSprint;
        }
    }
}