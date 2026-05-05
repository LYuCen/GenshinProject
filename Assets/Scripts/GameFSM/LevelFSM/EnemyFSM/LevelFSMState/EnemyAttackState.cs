using GameFSM.FSM;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState
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
            
        }

        public override void OnExitState()
        {
            MyManager.enemyAIController.ChangeMoveSpeed(3.5f);
        }
    }
}