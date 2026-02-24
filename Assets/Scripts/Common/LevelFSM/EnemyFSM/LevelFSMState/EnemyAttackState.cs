
using Common.FSM;
using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyAttackState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyAttackState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyIdleCondition(MyManager, "Idle"));
            AddCondition(new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterrupt"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "攻击决策";
            MyManager.enemyAIController.StopMove();
            MyManager.choiceTimer = MyManager.choiceCd - 0.5f; // 初始化决策时间
            MyManager.enemyAIController.ChangeMoveSpeed(1f);
        }

        public override void OnUpdateState()
        {
            Transform player = MyManager.GetPlayerTrans();
            if (!player ||
                !MyManager.enemyAIController.IsInRange(player, MyManager.outAttackRange))
            {
                MyManager.isAttack = false;
                return;
            }

            // 走位冷却
            MyManager.choiceTimer += Time.deltaTime;
            if (MyManager.choiceTimer >= MyManager.choiceCd)
            {
                // 只有太近才走位
                float distance = Vector3.Distance(
                    MyManager.transform.position, player.position);

                if (distance < MyManager.outAttackRange)
                {
                    MyManager.enemyAIController.StrafeAroundTarget(player, 2.0f);
                }

                MyManager.choiceTimer = 0f;
            }
            else MyManager.Shoot();
        }

        public override void OnExitState()
        {
            MyManager.enemyAIController.ChangeMoveSpeed(3.5f);
        }
    }
}