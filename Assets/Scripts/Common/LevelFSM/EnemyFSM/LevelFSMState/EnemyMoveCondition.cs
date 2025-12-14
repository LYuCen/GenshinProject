using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyMoveCondition : AbstractCondition<EnemyManager>
    {
        public EnemyMoveCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Move";
            return MyManager.isGround && MyManager.moveSpeed > 0.1f;
        }
    }
}