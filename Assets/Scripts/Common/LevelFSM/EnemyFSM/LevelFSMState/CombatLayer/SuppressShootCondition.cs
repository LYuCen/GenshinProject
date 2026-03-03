using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class SuppressShootCondition : AbstractCondition<EnemyManager>
    {
        public SuppressShootCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "SuppressShoot";
            return MyManager.isSuppressShoot;
        }
    }
}