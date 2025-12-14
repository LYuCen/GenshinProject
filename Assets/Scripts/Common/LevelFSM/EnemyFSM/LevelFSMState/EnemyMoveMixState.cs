namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyMoveMixState : AbstractMixLevelState<EnemyManager>
    {
        public EnemyMoveMixState(EnemyManager manager) : base(manager)
        {
            Conditions.Add(new EnemyIdleCondition(MyManager, "Idle"));
        }

        protected override void BuildState()
        {
            States.TryAdd("Walk", new EnemyWalkState(MyManager));
            States.TryAdd("Run", new EnemyRunState(MyManager));
            IsBuilt = true;
        }
        
        public override void OnEnterState()
        {
            MyManager.stateStr = "移动状态";
            SetDefaultState("Walk");
        }
    }
}