namespace Common.FSM.PlayerFSM.UpperBodyFSM
{
    public class ThrowGrenadeCondition : AbstractCondition<PlayerManager>
    {
        public ThrowGrenadeCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "ThrowGrenade";
            return MyManager.isThrow;
        }
    }
}