using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class MoveToCoverState : AbstractLevelState<EnemyManager>
    {
        
        public MoveToCoverState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new StrafeShootCondition(MyManager, "StrafeShoot"));
            AddCondition(new SuppressShootCondition(MyManager, "SuppressShoot"));
            AddCondition(new EnemyCombatIdleCondition(MyManager, "CombatIdle"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "寻找掩体";
        }

        public override void OnUpdateState()
        {

            if (MyManager.enemyAIController.HasArrived())
            {
                MyManager.isTryMoveCover = false;
            }
        }
    }
}