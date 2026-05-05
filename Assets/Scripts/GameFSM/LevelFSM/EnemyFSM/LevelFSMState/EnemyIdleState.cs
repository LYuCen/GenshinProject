using GameFSM.FSM;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyIdleState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyIdleState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterrupt"));
            AddCondition(new EnemyCombatLayerCondition(MyManager, "Combat"));

        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "待机状态";
        }

        public override void OnUpdateState()
        {
            
        }

        public override void OnExitState()
        {
            
        }
    }
}