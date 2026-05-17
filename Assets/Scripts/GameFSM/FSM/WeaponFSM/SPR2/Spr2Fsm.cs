using Firearms;

namespace GameFSM.FSM.WeaponFSM.SPR2
{
    public class Spr2Fsm : BaseFsm<AbstractGun>
    {
        public Spr2Fsm(AbstractStateBuilder<AbstractGun> stateBuilder) : base(stateBuilder)
        {
            SetDefaultState("Idle");
        }
    }
}