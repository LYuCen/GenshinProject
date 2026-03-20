using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer
{
    public class FindCoverCondition : AbstractCondition<EnemyManager>
    {
        public FindCoverCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "FindCover";
            return MyManager.GridEqsController.findCover;
        }
    }
}