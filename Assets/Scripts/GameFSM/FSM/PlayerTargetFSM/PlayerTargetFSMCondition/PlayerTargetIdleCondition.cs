using ShootingTarget;

namespace GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition
{
    /// <summary>
    /// 玩家待机 Idle
    /// </summary>
    public class PlayerTargetIdleCondition : AbstractCondition<PlayerTarget>
    {
        public PlayerTargetIdleCondition(PlayerTarget manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Idle";
            return MyManager.Data.playerMove.sqrMagnitude < 0.01f && !MyManager.Data.isCrouch &&
                   MyManager.Data.curHp > 0;
        }
    }
}