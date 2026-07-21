using ShootingTarget;

namespace GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition
{
    /// <summary>
    /// 标靶玩家行走 Walk
    /// </summary>
    public class PlayerTargetWalkCondition : AbstractCondition<PlayerTarget>
    {
        public PlayerTargetWalkCondition(PlayerTarget manager, string nextStateName) : base(manager, nextStateName)
        {
        }


        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            return MyManager.Data.playerMove.sqrMagnitude >= 0.01f && !MyManager.Data.isCrouch &&
                   MyManager.Data.curHp > 0;
        }
    }
}