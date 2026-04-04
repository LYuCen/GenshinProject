namespace GameFSM.FSM.PlayerFSM.UpperBodyFSM
{
    public class UpperBodyFsm : BaseFsm<PlayerManager>
    {
        public UpperBodyFsm(AbstractStateBuilder<PlayerManager> stateBuilder) : base(stateBuilder)
        {
            SetDefaultState("Null");
        }
    }
}