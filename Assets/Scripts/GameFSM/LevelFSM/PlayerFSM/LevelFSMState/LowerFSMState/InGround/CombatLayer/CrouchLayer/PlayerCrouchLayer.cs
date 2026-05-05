using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer
{
    /// <summary>
    /// 玩家蹲姿状态层
    /// </summary>
    public class PlayerCrouchLayer : AbstractMixLevelState<PlayerManager>
    {
        public PlayerCrouchLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerStandLayerCondition(MyManager, "Stand"));
        }

        protected override void BuildState()
        {
            States.Add("Walk", new PlayerCrouchWalkState(MyManager, new PlayerCrouchWalkStateCondition(MyManager, "Walk")));
            States.Add("Sprint", new PlayerCrouchSprintState(MyManager, new PlayerCrouchSprintStateCondition(MyManager, "Sprint")));
            IsBuilt = true;
        }
    }
}