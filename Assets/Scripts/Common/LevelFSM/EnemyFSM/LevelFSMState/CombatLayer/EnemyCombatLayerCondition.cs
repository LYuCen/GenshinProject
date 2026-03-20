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
            nextState = "CombatLayer";
            return MyManager.isCombat && !MyManager.isGlobalInterrupt
                                      && MyManager.bodyController.hp >= MyManager.bodyController.maxHp * 0.3;
        }
    }
}