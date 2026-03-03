using Common.FSM;
using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class EnemyCombatLayer : AbstractMixLevelState<EnemyManager>
    {
        public EnemyCombatLayer(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyGlobalInterruptLayerCondition(MyManager,"GlobalInterrupt"));
        }

        protected override void BuildState()
        {
            States.TryAdd("MoveToCover",
                new MoveToCoverState(MyManager, new MoveToCoverStateCondition(MyManager, "MoveToCover")));
            States.TryAdd("StrafeShoot",
                new StrafeShootState(MyManager, new StrafeShootCondition(MyManager, "StrafeShoot")));
            States.TryAdd("SuppressShoot",
                new SuppressShootState(MyManager, new StrafeShootCondition(MyManager, "SuppressShoot")));
            States.TryAdd("CombatIdle",
                new EnemyCombatIdleState(MyManager, new EnemyCombatIdleCondition(MyManager, "CombatIdle")));
            IsBuilt = true;
        }
    }
}