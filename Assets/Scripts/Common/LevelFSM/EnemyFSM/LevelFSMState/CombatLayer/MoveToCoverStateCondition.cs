using Common.FSM;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer
{
    public class MoveToCoverStateCondition : AbstractCondition<EnemyManager>
    {
        public MoveToCoverStateCondition(EnemyManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "MoveToCover";
            return MyManager.isTryMoveCover;
        }
    }
}