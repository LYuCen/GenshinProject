using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyIdleState : AbstractLeafLevelState<EnemyManager>
    {
        
        public EnemyIdleState(EnemyManager manager) : base(manager)
        {
            Conditions.Add(new EnemyAttackCondition(MyManager,"Attack"));
            Conditions.Add(new EnemyFollowCondition(MyManager, "Follow"));
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