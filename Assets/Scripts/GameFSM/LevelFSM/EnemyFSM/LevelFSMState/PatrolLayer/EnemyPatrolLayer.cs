using GameFSM.FSM;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer
{
    /// <summary>
    /// 巡逻层
    /// </summary>
    public class EnemyPatrolLayer : AbstractMixLevelState<EnemyManager>
    {
        public EnemyPatrolLayer(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyCombatLayerCondition(MyManager,"CombatLayer"));
            AddCondition(new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterruptLayer"));
            AddCondition(new EnemyRetreatLayerCondition(MyManager, "RetreatLayer"));
        }

        protected override void BuildState()
        {
            States.Add("PatrolIdle",
                new EnemyPatrolIdleState(MyManager, new EnemyPatrolIdleCondition(MyManager, "PatrolIdle")));
            States.Add("PatrolWalk",
                new EnemyPatrolWalkState(MyManager, new EnemyPatrolWalkCondition(MyManager, "PatrolWalk")));
            IsBuilt = true;
        }

        public override void OnEnterState()
        {
            if(MyManager.EnemyPatrol) 
                MyManager.EnemyPatrol.enable = true;
        }

        public override void OnUpdateState()
        {
            MyManager.isCombat = MyManager.AISightPerception.isSight;
            MyManager.enemyAIController.RotateToMoveDirection(8f);
            base.OnUpdateState();
        }

        public override void OnExitState()
        {
            if(MyManager.EnemyPatrol) 
                MyManager.EnemyPatrol.enable = false;
            MyManager.enemyAIController.StopMove();
        }
    }
}