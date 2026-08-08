using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir
{
    /// <summary>
    /// 玩家跳跃状态判定 Jump
    /// </summary>
    public class PlayerJumpStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerJumpStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Jump";
            return MyManager.state.isJump;
        }
    }
}