using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class StrafeShootCondition : AbstractCondition<EnemyManager>
    {
        public StrafeShootCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
            
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "StrafeShoot";
            return MyManager.isStrafeShoot;
        }
    }
}