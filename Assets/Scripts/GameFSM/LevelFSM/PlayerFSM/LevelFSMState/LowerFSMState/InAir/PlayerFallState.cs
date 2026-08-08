using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir
{
    public class PlayerFallState : AbstractLevelState<PlayerManager>
    {
        public PlayerFallState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerJumpStateCondition(MyManager, "Jump"));
        }
    }
}