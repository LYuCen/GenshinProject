using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer
{
    public class EnemyPatrolIdleState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyPatrolIdleState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyPatrolWalkCondition(MyManager,"PatrolWalk"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "巡逻待机状态";
        }

        public override void OnUpdateState()
        {
        }
    }
}