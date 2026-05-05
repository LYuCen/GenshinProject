using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer
{
    public class EnemyPatrolWalkCondition : AbstractCondition<EnemyManager>
    {
        public EnemyPatrolWalkCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "PatrolWalk";
            return MyManager.moveSpeed > 0.1f;
        }
    }
}