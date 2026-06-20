using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.GlobalInterruptState;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround
{
    /// <summary>
    /// 玩家地面状态层
    /// </summary>
    public class PlayerGroundLayer : AbstractMixLevelState<PlayerManager>
    {
        public PlayerGroundLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerAirLayerCondition(MyManager, "Air"));
            AddCondition(new GlobalInterruptLayerCondition(MyManager, "GlobalInterrupt"));
        }

        protected override void BuildState()
        {
            States.Add("Idle", new PlayerIdleState(MyManager, new PlayerIdleStateCondition(MyManager, "Idle")));
            States.Add("Combat", new PlayerCombatLayer(MyManager, new PlayerCombatLayerCondition(MyManager, "Combat")));
            IsBuilt = true;
        }
    }
}