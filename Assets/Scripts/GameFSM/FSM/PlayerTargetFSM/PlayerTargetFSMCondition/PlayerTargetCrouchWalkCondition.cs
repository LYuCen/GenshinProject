using ShootingTarget;

namespace GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition
{
    /// <summary>
    /// 标靶玩家蹲姿行走 CrouchWalk
    /// </summary>
    public class PlayerTargetCrouchWalkCondition : AbstractCondition<PlayerTarget>
    {
        public PlayerTargetCrouchWalkCondition(PlayerTarget manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "CrouchWalk";
            return MyManager.Data.playerMove.sqrMagnitude >= 0.01f && MyManager.Data.isCrouch &&
                   MyManager.Data.curHp > 0;
        }
    }
}