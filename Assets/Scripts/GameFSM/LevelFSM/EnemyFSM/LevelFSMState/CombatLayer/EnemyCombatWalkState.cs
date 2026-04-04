using GameFSM.FSM;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class EnemyCombatWalkState : AbstractLevelState<EnemyManager>
    {
        private float _waitTime = 0.5f;
        private float _timer;
        
        public EnemyCombatWalkState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyCombatIdleCondition(MyManager, "CombatIdle"));
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "战斗移动";
            _timer = Time.time + _waitTime;
        }

        public override void OnUpdateState()
        {
            if(Time.time > _timer) MyManager.Shoot();
        }
    }
}