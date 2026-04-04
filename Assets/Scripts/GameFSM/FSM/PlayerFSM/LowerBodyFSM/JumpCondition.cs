namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class JumpCondition : AbstractCondition<PlayerManager>
    {
        public JumpCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
            
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Jump";
            return MyManager.IsGround && MyManager.IsJump;
        }
    }
}