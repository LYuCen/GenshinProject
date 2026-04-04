using GameFSM.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer
{
    public class EnemyDeadCondition : AbstractCondition<EnemyManager>
    {
        public EnemyDeadCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Dead";
            return MyManager.bodyController.hp <= 0;
        }
    }
}