using Common.FSM;
using Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer
{
    /// <summary>
    /// 全局打断层, 满足状态条件强制打断其他状态
    /// </summary>
    public class EnemyGlobalInterruptLayer : AbstractMixLevelState<EnemyManager>
    {
        public EnemyGlobalInterruptLayer(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyCombatLayerCondition(MyManager, "Combat"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "强制控制状态";
        }

        protected override void BuildState()
        {
            States.TryAdd("Dead", new EnemyDeadState(MyManager, new EnemyDeadCondition(MyManager, "Dead")));
            IsBuilt = true;
        }
    }
}