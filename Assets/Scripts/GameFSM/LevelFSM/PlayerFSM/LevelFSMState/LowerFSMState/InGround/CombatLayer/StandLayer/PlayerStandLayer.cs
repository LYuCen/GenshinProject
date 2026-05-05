using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer
{
    /// <summary>
    /// 玩家站立状态层
    /// </summary>
    public class PlayerStandLayer : AbstractMixLevelState<PlayerManager>
    {
        public PlayerStandLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerCrouchLayerCondition(MyManager, "Crouch"));
        }

        protected override void BuildState()
        {
            States.Add("Walk", new PlayerStandWalkState(MyManager, new PlayerStandWalkStateCondition(MyManager, "Walk")));
            States.Add("Sprint", new PlayerStandSprintState(MyManager, new PlayerStandSprintStateCondition(MyManager, "Sprint")));
            IsBuilt = true;
        }
    }
}