using Common.FSM;
using Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer;
using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;
using Common.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer;
using Common.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class RootState : AbstractMixLevelState<EnemyManager>
    {
        public RootState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            
        }

        protected override void BuildState()
        {
            States.Add("PatrolLayer",
                new EnemyPatrolLayer(MyManager, new EnemyPatrolLayerCondition(MyManager, "PatrolLayer")));
            States.Add("CombatLayer",
                new EnemyCombatLayer(MyManager, new EnemyCombatLayerCondition(MyManager, "CombatLayer")));
            States.Add("GlobalInterruptLayer",
                new EnemyGlobalInterruptLayer(MyManager,
                    new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterruptLayer")));
            States.Add("RetreatLayer",
                new EnemyRetreatLayer(MyManager, new EnemyRetreatLayerCondition(MyManager, "RetreatLayer")));
            IsBuilt = true;
        }
    }
}