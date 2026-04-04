namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class SlidingCondition : AbstractCondition<PlayerManager>
    {
        public SlidingCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Sliding";
            return MyManager.IsGround && MyManager.IsSliding;
        }
    }
}