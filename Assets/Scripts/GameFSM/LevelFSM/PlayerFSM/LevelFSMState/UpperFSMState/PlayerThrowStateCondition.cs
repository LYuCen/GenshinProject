using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家投掷状态判定 (Throw)
    /// </summary>
    public class PlayerThrowStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerThrowStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Throw";
            return MyManager.IsThrow;
        }
    }
}