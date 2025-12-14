using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyWalkState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyWalkState(EnemyManager manager) : base(manager)
        {
            Conditions.Add(new EnemyRunCondition(MyManager,"Run"));
        }
        
        public override void OnEnterState()
        {
            MyManager.stateStr = "行走状态";
        }
    }
}