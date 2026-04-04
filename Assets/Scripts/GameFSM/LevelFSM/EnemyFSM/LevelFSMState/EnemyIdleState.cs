using Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer;
using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;
using GameFSM.FSM;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
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