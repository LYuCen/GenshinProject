using Common.FSM;
using mSystem.AISystem;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class MoveToCoverState : AbstractLevelState<EnemyManager>
    {
        private CoverPoint _coverPoint;
        
        public MoveToCoverState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new StrafeShootCondition(MyManager, "StrafeShoot"));
            AddCondition(new SuppressShootCondition(MyManager, "SuppressShoot"));
            AddCondition(new EnemyCombatIdleCondition(MyManager, "CombatIdle"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "寻找掩体";
            CoverPoint[] points = MyManager.GetCoverPoints();
            if(points.Length <= 0) return;
            for (int i = 0; i < points.Length; i++)
            {
                if (!points[i].isOccupied)
                {
                    _coverPoint = points[i];
                    MyManager.enemyAIController.MoveToAim(_coverPoint.transform);
                    _coverPoint.isOccupied = true;
                    break;
                }
            }
        }

        public override void OnUpdateState()
        {
            if (!_coverPoint) return;

            if (MyManager.enemyAIController.HasArrived())
            {
                MyManager.isTryMoveCover = false;
            }
        }
    }
}