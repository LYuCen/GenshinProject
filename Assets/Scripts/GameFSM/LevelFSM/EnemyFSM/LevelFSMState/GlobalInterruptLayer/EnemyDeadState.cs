using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer
{
    /// <summary>
    /// 敌人死亡状态
    /// </summary>
    public class EnemyDeadState : AbstractLeafLevelState<EnemyManager>
    {
        public EnemyDeadState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
        }

        public override void OnEnterState()
        {
            Debug.Log("敌人已阵亡");
            MyManager.CreateSnapshot();
            MyManager.gameObject.SetActive(false);
        }
    }
}