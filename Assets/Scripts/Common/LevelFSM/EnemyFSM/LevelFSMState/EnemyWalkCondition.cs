using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyWalkCondition : AbstractCondition<EnemyManager>
    {
        public EnemyWalkCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            return MyManager.isGround && MyManager.moveSpeed < 2.0f;
        }
    }
}