namespace GameFSM.FSM.PlayerFSM.UpperBodyFSM
{
    public class ReloadCondition : AbstractCondition<PlayerManager>
    {
        public ReloadCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }


        public override bool CheckCondition(out string nextState)
        {
            nextState = "Reload";
            return MyManager.IsReload;
        }
    }
}