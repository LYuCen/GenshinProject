using System.Collections.Generic;
using Common.FSM;
using mSystem.EQSSystem;
using mSystem.EQSSystem.EqsTest;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer
{
    /// <summary>
    /// 后退射击（未发现掩体时）
    /// </summary>
    public class RetreatCombatState : AbstractLeafLevelState<EnemyManager>
    {
        private List<IEqsTest> _tests;
        private InCircleTest _inCircleTest;
        public RetreatCombatState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new FindCoverCondition(MyManager, "FindCover"));

            _tests = new List<IEqsTest>();
            _tests.Add(new NavMeshReachableTest());
            _inCircleTest = new InCircleTest(MyManager.transform.position, 2f, 1, true, true);
            _tests.Add(_inCircleTest);
            _tests.Add(new DistanceToPositionTest(MyManager.GridEqsController.target.position, 1f, true));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "且战且退状态";
            MyManager.GridEqsController.SetEqsSystemTest(_tests);
            MyManager.isCombat = true;
        }

        public override void OnUpdateState()
        {
            _inCircleTest.Center = MyManager.transform.position;
            Vector3 forward = MyManager.GridEqsController.target.position - MyManager.transform.position;
            MyManager.enemyAIController.RotateToMoveDirection(8f, forward);
            EqsPoint bestPoint = MyManager.GridEqsController.BestPoint;
            if (bestPoint != null)
            {
                MyManager.enemyAIController.MoveToAim(bestPoint.Position,true);
            }
            MyManager.Shoot();
            
        }

        public override void OnExitState()
        {
            MyManager.isCombat = false;
        }
    }
}