using Firearms;

namespace GameFSM.FSM.WeaponFSM.FN_IWS
{
    public class FnIwsFsm : BaseFsm<AbstractGun>
    {
        public FnIwsFsm(AbstractStateBuilder<AbstractGun> stateBuilder) : base(stateBuilder)
        {
            SetDefaultState("Idle");
        }
    }
}