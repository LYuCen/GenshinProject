using GameFSM.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyAttackCondition : AbstractCondition<EnemyManager>
    {
        public EnemyAttackCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Attack";
            return MyManager.isGround && MyManager.isAttack;
        }
    }
}