using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyRunCondition : AbstractCondition<EnemyManager>
    {
        public EnemyRunCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Run";
            return MyManager.isGround && MyManager.moveSpeed > 2.0f;
        }
    }
}