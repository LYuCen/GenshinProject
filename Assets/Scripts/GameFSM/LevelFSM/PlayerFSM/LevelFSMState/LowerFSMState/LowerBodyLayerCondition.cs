using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState
{
    /// <summary>
    /// 角色下半身层判定 (Lower)
    /// </summary>
    public class LowerBodyLayerCondition : AbstractCondition<PlayerManager>
    {
        public LowerBodyLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Lower";
            return true;
        }
    }
}