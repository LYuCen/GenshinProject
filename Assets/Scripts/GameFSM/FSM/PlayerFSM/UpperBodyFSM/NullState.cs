using GameFSM.LevelFSM.PlayerFSM;

namespace GameFSM.FSM.PlayerFSM.UpperBodyFSM
{
    public class NullState : AbstractState<PlayerManager>
    {
        public NullState(PlayerManager manager) : base(manager)
        {
            AddCondition(new ReloadCondition(MyManager, "Reload"));
            AddCondition(new ThrowGrenadeCondition(MyManager, "ThrowGrenade"));
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