namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyIdleState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyIdleState(EnemyManager manager) : base(manager)
        {
            Conditions.Add(new EnemyMoveCondition(MyManager, "Move"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "待机状态";
        }
    }
}