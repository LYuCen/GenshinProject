namespace Common.FSM.PlayerFSM.UpperBodyFSM
{
    public class ThrowGrenadeState : AbstractState<PlayerManager>
    {
        public ThrowGrenadeState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new NullCondition(MyManager,"Null"));
        }
        
        public override void OnEnterState()
        {
            MyManager.upperName = "投弹";
            MyManager.PlayAnimController.OpenIk = false;
        }

        public override void OnUpdateState()
        {
            
        }

        public override void OnExitState()
        {
            MyManager.PlayAnimController.OpenIk = true;
        }
    }
}