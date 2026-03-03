using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class EnemyCombatLayerCondition : AbstractCondition<EnemyManager>
    {
        public EnemyCombatLayerCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }


        public override bool CheckCondition(out string nextState)
        {
            nextState = "Combat";
            return MyManager.isCombat;
        }
    }
}