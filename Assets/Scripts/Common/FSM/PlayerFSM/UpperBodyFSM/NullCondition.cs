namespace Common.FSM.PlayerFSM.UpperBodyFSM
{
    public class NullCondition : AbstractCondition<PlayerManager>
    {
        public NullCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
            
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Null";
            return !MyManager.isReload && !MyManager.isThrow;
        }
    }
}