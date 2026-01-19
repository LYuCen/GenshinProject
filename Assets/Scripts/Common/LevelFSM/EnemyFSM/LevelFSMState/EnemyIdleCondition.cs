using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyIdleCondition : AbstractCondition<EnemyManager>
    {
        public EnemyIdleCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Idle";
            return MyManager.isGround && MyManager.moveSpeed < 0.01f && !MyManager.isFollow && !MyManager.isAttack;
        }
    }
}