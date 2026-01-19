using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyFollowCondition : AbstractCondition<EnemyManager>
    {
        public EnemyFollowCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Follow";
            return MyManager.isGround && MyManager.isFollow && !MyManager.isAttack;
        }
    }
}