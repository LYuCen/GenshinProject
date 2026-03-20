using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class EnemyCombatWalkCondition : AbstractCondition<EnemyManager>
    {
        public EnemyCombatWalkCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "CombatWalk";
            return MyManager.moveSpeed > 0.5f;
        }
    }
}