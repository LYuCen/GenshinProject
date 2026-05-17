using Firearms;

namespace GameFSM.FSM.WeaponFSM.FN_IWS.State
{
    public class FnIwsIdleState : AbstractState<AbstractGun>
    {
        public FnIwsIdleState(AbstractGun manager) : base(manager)
        {
            AddCondition(new FnIwsReloadCondition(MyManager, "Reload"));
            AddCondition(new FnIwsShootCondition(MyManager, "Shoot"));
        }

        public override void OnEnterState()
        {
        }

        public override void OnUpdateState()
        {
        }
    }
}