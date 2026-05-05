using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer
{
    public class EnemyGlobalInterruptLayerCondition : AbstractCondition<EnemyManager>
    {
        public EnemyGlobalInterruptLayerCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "GlobalInterruptLayer";
            return MyManager.isGlobalInterrupt;
        }
    }
}