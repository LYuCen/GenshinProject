using Common.FSM;
using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyFollowState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyFollowState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyIdleCondition(MyManager, "Idle"));
            AddCondition(new EnemyAttackCondition(MyManager, "Attack"));
            AddCondition(new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterrupt"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "追击决策";
            
        }

        public override void OnUpdateState()
        {
            Transform player = MyManager.GetPlayerTrans();
            if (player)
            {
                if (!MyManager.enemyAIController.IsInRange(player, MyManager.attackRange))
                {
                    MyManager.enemyAIController.MoveToAim(player, true);
                }
                else MyManager.isAttack = true;
            }
        }

        public override void OnExitState()
        {
            MyManager.isFollow = false;
        }
    }
}