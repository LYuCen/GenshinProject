using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer
{
    /// <summary>
    /// 掩体后待机
    /// </summary>
    public class CoverIdleState : AbstractLeafLevelState<EnemyManager>
    {
        public CoverIdleState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager,
            selfCondition)
        {
            AddCondition(new RetreatCombatCondition(MyManager, "RetreatCombat"));
            AddCondition(new FindCoverCondition(MyManager, "FindCover"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "掩体躲藏待机状态";
        }
    }
}