using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyFollowState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyFollowState(EnemyManager manager) : base(manager)
        {
            Conditions.Add(new EnemyIdleCondition(MyManager, "Idle"));
            Conditions.Add(new EnemyAttackCondition(MyManager, "Attack"));
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