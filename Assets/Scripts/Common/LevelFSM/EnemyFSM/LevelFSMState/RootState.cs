namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class RootState : AbstractMixLevelState<EnemyManager>
    {
        public RootState(EnemyManager manager) : base(manager)
        {
            SetDefaultState("Idle");
        }

        protected override void BuildState()
        {
            States.TryAdd("Idle", new EnemyIdleState(MyManager));
            States.TryAdd("Follow", new EnemyFollowState(MyManager));
            States.TryAdd("Attack", new EnemyAttackState(MyManager));
            IsBuilt = true;
        }
    }
}