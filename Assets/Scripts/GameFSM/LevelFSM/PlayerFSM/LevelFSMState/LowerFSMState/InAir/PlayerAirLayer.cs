using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.GlobalInterruptState;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir
{
    /// <summary>
    /// 玩家滞空层
    /// </summary>
    public class PlayerAirLayer : AbstractMixLevelState<PlayerManager>
    {
        public PlayerAirLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerGroundLayerCondition(MyManager, "Ground"));
            AddCondition(new GlobalInterruptLayerCondition(MyManager, "GlobalInterrupt"));
        }

        protected override void BuildState()
        {
            States.Add("Jump", new PlayerJumpState(MyManager, new PlayerJumpStateCondition(MyManager, "Jump")));
            States.Add("Fall", new PlayerFallState(MyManager, new PlayerFallStateCondition(MyManager, "Fall")));
            IsBuilt = true;
        }
    }
}