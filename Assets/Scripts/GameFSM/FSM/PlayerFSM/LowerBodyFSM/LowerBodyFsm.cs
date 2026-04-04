namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class LowerBodyFsm : BaseFsm<PlayerManager>
    {
        public LowerBodyFsm(AbstractStateBuilder<PlayerManager> stateBuilder) : base(stateBuilder)
        {
            SetDefaultState("Idle");
        }
    }
}