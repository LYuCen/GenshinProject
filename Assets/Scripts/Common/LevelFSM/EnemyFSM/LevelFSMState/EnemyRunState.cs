namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyRunState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyRunState(EnemyManager manager) : base(manager)
        {
            Conditions.Add(new EnemyWalkCondition(MyManager,"Walk"));
        }
        
        public override void OnEnterState()
        {
            MyManager.stateStr = "奔跑状态";
        }
    }
}