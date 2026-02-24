using Common.FSM;
using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyIdleState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyIdleState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyAttackCondition(MyManager,"Attack"));
            AddCondition(new EnemyFollowCondition(MyManager, "Follow"));
            AddCondition(new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterrupt"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "待机状态";
        }

        public override void OnUpdateState()
        {
            Transform player = MyManager.GetPlayerTrans();
            if (MyManager.enemyAIController.IsInRange(player, MyManager.perceptionRange))
            {
                // 进入追击状态
                MyManager.isFollow = true;
            }
        }

        public override void OnExitState()
        {
            
        }
    }
}