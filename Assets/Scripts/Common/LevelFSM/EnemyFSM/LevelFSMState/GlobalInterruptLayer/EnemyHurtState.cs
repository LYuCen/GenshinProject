using Common.FSM;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer
{
    public class EnemyHurtState : AbstractLevelState<EnemyManager>
    {
        private float _waitTime = 0.2f;
        private float _timer;
        
        public EnemyHurtState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyDeadCondition(MyManager, "Dead"));
        }

        public override void OnEnterState()
        {
            _timer = Time.time + _waitTime;
            Debug.Log("敌人受击");
        }

        public override void OnUpdateState()
        {
            if (Time.time > _timer)
            {
                MyManager.isHurt = false;
                MyManager.isGlobalInterrupt = false;
            }
        }
    }
}