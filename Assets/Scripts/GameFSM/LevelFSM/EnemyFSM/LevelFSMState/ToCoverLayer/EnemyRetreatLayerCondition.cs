using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer
{
    public class EnemyRetreatLayerCondition : AbstractCondition<EnemyManager>
    {
        public EnemyRetreatLayerCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "RetreatLayer";
            // 血量低于 30%
            return MyManager.bodyController.hp < MyManager.bodyController.maxHp * 0.3
                   && !MyManager.isGlobalInterrupt;
        }
    }
}