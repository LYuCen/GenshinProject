using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer
{
    public class EnemyPatrolIdleCondition : AbstractCondition<EnemyManager>
    {
        public EnemyPatrolIdleCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "PatrolIdle";
            return MyManager.moveSpeed <= 0.1f;
        }
    }
}