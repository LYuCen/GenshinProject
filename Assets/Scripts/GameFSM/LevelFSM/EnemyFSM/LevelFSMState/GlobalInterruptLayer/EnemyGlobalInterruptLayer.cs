using GameFSM.FSM;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer
{
    /// <summary>
    /// 全局打断层, 满足状态条件强制打断其他状态
    /// </summary>
    public class EnemyGlobalInterruptLayer : AbstractMixLevelState<EnemyManager>
    {
        public EnemyGlobalInterruptLayer(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyCombatLayerCondition(MyManager, "Combat"));
            AddCondition(new EnemyPatrolLayerCondition(MyManager,"PatrolLayer"));
            AddCondition(new EnemyRetreatLayerCondition(MyManager, "RetreatLayer"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "强制控制状态";
        }

        protected override void BuildState()
        {
            States.TryAdd("Dead", new EnemyDeadState(MyManager, new EnemyDeadCondition(MyManager, "Dead")));
            States.TryAdd("Hurt", new EnemyHurtState(MyManager, new EnemyHurtCondition(MyManager, "Hurt")));
            IsBuilt = true;
        }
    }
}