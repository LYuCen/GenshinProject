using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer
{
    public class EnemyHurtCondition : AbstractCondition<EnemyManager>
    {
        public EnemyHurtCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Hurt";
            return MyManager.isHurt;
        }
    }
}