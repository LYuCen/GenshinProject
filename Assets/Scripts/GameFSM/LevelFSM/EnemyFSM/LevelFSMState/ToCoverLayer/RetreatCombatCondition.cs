using GameFSM.FSM;

namespace GameFSM.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer
{
    public class RetreatCombatCondition : AbstractCondition<EnemyManager>
    {
        public RetreatCombatCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "RetreatCombat";
            return !MyManager.GridEqsController.findCover && MyManager.AISightPerception.playerCanSee;
        }
    }
}