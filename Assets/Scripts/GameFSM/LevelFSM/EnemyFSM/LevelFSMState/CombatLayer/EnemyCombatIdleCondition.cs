using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class EnemyCombatIdleCondition : AbstractCondition<EnemyManager>
    {
        public EnemyCombatIdleCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "CombatIdle";
            return MyManager.moveSpeed <= 0.1f;
        }
    }
}