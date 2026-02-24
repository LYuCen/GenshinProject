using Common.FSM;
using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class RootState : AbstractMixLevelState<EnemyManager>
    {
        public RootState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            SetDefaultState("Idle");
        }

        protected override void BuildState()
        {
            States.TryAdd("Idle", new EnemyIdleState(MyManager, new EnemyIdleCondition(MyManager, "Idle")));
            States.TryAdd("Follow", new EnemyFollowState(MyManager, new EnemyFollowCondition(MyManager, "Follow")));
            States.TryAdd("Attack", new EnemyAttackState(MyManager, new EnemyAttackCondition(MyManager, "Attack")));
            States.TryAdd("GlobalInterrupt",
                new EnemyGlobalInterruptLayer(MyManager,
                    new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterrupt")));
            IsBuilt = true;
        }
    }
}