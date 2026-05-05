using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer
{
    public class EnemyPatrolLayerCondition : AbstractCondition<EnemyManager>
    {
        public EnemyPatrolLayerCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "PatrolLayer";
            return !MyManager.isCombat && !MyManager.isGlobalInterrupt
                                       && MyManager.bodyController.hp >= MyManager.bodyController.maxHp * 0.3;
        }
    }
}