namespace Common.FSM.PlayerFSM.UpperBodyFSM
{
    public class NullState : AbstractState<PlayerManager>
    {
        public NullState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new ReloadCondition(MyManager, "Reload"));
            Conditions.Add(new ThrowGrenadeCondition(MyManager, "ThrowGrenade"));
        }

        public override void OnEnterState()
        {
            MyManager.upperName = "无状态";
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
        }
    }
}