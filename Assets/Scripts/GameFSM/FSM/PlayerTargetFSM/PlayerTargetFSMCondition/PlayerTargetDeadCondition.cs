using ShootingTarget;

namespace GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition
{
    /// <summary>
    /// 靶子玩家死亡 Dead
    /// </summary>
    public class PlayerTargetDeadCondition : AbstractCondition<PlayerTarget>
    {
        public PlayerTargetDeadCondition(PlayerTarget manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Dead";
            return MyManager.Data.curHp <= 0;
        }
    }
}