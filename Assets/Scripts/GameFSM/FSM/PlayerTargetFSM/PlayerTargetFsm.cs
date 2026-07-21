namespace GameFSM.FSM.PlayerTargetFSM
{
    public class PlayerTargetFsm : BaseFsm<ShootingTarget.PlayerTarget>
    {
        public PlayerTargetFsm(AbstractStateBuilder<ShootingTarget.PlayerTarget> stateBuilder) : base(stateBuilder)
        {
            SetDefaultState("Idle");
        }
    }
}