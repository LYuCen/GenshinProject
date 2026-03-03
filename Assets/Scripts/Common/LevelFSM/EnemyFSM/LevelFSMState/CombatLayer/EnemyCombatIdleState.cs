using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class EnemyCombatIdleState : AbstractLevelState<EnemyManager>
    {
        public EnemyCombatIdleState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveToCoverStateCondition(MyManager,"MoveToCover"));
            AddCondition(new StrafeShootCondition(MyManager, "StrafeShoot"));
            AddCondition(new SuppressShootCondition(MyManager, "SuppressShoot"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "战斗待机状态";
        }
    }
}