using ShootingTarget;

namespace GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition
{
    /// <summary>
    /// 标靶玩家蹲姿待机 CrouchIdle
    /// </summary>
    public class PlayerTargetCrouchIdleCondition : AbstractCondition<PlayerTarget>
    {
        public PlayerTargetCrouchIdleCondition(PlayerTarget manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "CrouchIdle";
            return MyManager.Data.playerMove.sqrMagnitude < 0.01f && MyManager.Data.isCrouch &&
                   MyManager.Data.curHp > 0;
        }
    }
}