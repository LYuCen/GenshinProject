using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer
{
    public class EnemyPatrolWalkState : AbstractLevelState<EnemyManager>
    {
        public EnemyPatrolWalkState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyPatrolIdleCondition(MyManager,"PatrolIdle"));
        }
        
        public override void OnEnterState()
        {
            MyManager.stateStr = "巡逻行走状态";
        }

        public override void OnUpdateState()
        {
        }
    }
}