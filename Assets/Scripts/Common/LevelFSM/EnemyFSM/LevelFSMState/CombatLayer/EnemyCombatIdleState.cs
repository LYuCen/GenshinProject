using Common.FSM;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class EnemyCombatIdleState : AbstractLevelState<EnemyManager>
    {
        private float _waitTime = 1f;
        private float _timer;
        public EnemyCombatIdleState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyCombatWalkCondition(MyManager,"CombatWalk"));
        }

        public override void OnEnterState()
        {
            _timer = Time.time + _waitTime;
            MyManager.stateStr = "战斗待机状态";
        }

        public override void OnUpdateState()
        {
            if (Time.time > _timer)
            {
                MyManager.enemyAIController.MoveToAim(MyManager.CircleEqsControllerTest.BasePoint.Position, true);
            }
        }
    }
}