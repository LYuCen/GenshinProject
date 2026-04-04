using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;
using Common.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer;
using Common.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer;
using GameFSM.FSM;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    /// <summary>
    /// 战斗层
    /// </summary>
    public class EnemyCombatLayer : AbstractMixLevelState<EnemyManager>
    {
        public EnemyCombatLayer(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyPatrolLayerCondition(MyManager,"PatrolLayer"));
            AddCondition(new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterruptLayer"));
            AddCondition(new EnemyRetreatLayerCondition(MyManager, "RetreatLayer"));
        }

        protected override void BuildState()
        {
            States.Add("CombatWalk",
                new EnemyCombatWalkState(MyManager, new EnemyAttackCondition(MyManager, "CombatWalk")));
            States.Add("CombatIdle",
                new EnemyCombatIdleState(MyManager, new EnemyCombatIdleCondition(MyManager, "CombatWalk")));
            IsBuilt = true;
        }

        public override void OnUpdateState()
        {
            MyManager.isCombat = MyManager.AISightPerception.isSight;
            MyManager.CircleEqsControllerTest.center = MyManager.AISightPerception.sightPoint;
            MyManager.CircleEqsControllerTest.enable = true;
            Vector3 forward = MyManager.CircleEqsControllerTest.center - MyManager.transform.position;
            MyManager.enemyAIController.RotateToMoveDirection(8f, forward);
            base.OnUpdateState();
        }

        public override void OnExitState()
        {
            MyManager.CircleEqsControllerTest.enable = false;
        }
    }
}