using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir
{
    /// <summary>
    /// 玩家坠落判定 Fall
    /// </summary>
    public class PlayerFallStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerFallStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Fall";
            return !MyManager.state.isJump;
        }
    }
}