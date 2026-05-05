using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer
{
    /// <summary>
    /// 寻找掩体
    /// </summary>
    public class FindCoverState : AbstractLeafLevelState<EnemyManager>
    {
        public FindCoverState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager,
            selfCondition)
        {
            //AddCondition(new CoverIdleCondition(MyManager, "CoverIdle"));
            AddCondition(new RetreatCombatCondition(MyManager, "RetreatCombat"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "寻找掩体";
            MyManager.GridEqsController.DistanceToPositionTest.IsOpposite = false;
            MyManager.GridEqsController.SetEqsSystemTest(null);

        }

        public override void OnUpdateState()
        {
            if (MyManager.GridEqsController.BestPoint != null)
            {
                MyManager.enemyAIController.MoveToAim(MyManager.GridEqsController.BestPoint.Position, true);
            }
            Vector3 dir = MyManager.enemyAIController.GetAgent().desiredVelocity;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                MyManager.transform.rotation = Quaternion.Slerp(
                    MyManager.transform.rotation,
                    targetRot,
                    Time.deltaTime * 10f
                );
            }
        }
    }
}