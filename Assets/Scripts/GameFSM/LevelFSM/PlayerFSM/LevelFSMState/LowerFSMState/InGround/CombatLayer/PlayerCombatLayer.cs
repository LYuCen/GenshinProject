using Common.LevelFSM;
using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer
{
    public class PlayerCombatLayer : AbstractMixLevelState<PlayerManager>
    {
        public PlayerCombatLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerIdleStateCondition(MyManager, "Idle"));
        }

        protected override void BuildState()
        {
            States.Add("Stand", new PlayerStandLayer(MyManager, new PlayerStandLayerCondition(MyManager, "Stand")));
            States.Add("Crouch", new PlayerCrouchLayer(MyManager, new PlayerCrouchLayerCondition(MyManager, "Crouch")));
            IsBuilt = true;
        }
    }
}