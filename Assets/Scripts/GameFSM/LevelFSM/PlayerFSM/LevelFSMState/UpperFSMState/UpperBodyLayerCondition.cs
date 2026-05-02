using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家上半身层判定 (Upper)
    /// </summary>
    public class UpperBodyLayerCondition : AbstractCondition<PlayerManager>
    {
        public UpperBodyLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Upper";
            return true;
        }
    }
}