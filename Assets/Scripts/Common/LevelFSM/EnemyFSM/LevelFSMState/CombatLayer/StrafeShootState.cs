using Common.FSM;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class StrafeShootState : AbstractLevelState<EnemyManager>
    {
        
        public StrafeShootState(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveToCoverStateCondition(MyManager, "MoveToCover"));
        }
        
        public override void OnEnterState()
        {
            MyManager.stateStr = "转移射击状态";
            
        }

        public override void OnUpdateState()
        {
            Transform player = MyManager.GetPlayerTrans();
            if (!player ||
                !MyManager.enemyAIController.IsInRange(player, MyManager.outAttackRange))
            {
                MyManager.isAttack = false;
                return;
            }
            
            

            
            MyManager.Shoot();
        }

        public override void OnExitState()
        {
            
        }
    }
}