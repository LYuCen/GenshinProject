using GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition;

namespace GameFSM.FSM.PlayerTargetFSM.State
{
    public class PlayerTargetCrouchWalkState : AbstractState<ShootingTarget.PlayerTarget>
    {
        public PlayerTargetCrouchWalkState(ShootingTarget.PlayerTarget manager) : base(manager)
        {
            AddCondition(new PlayerTargetWalkCondition(MyManager, "Walk"));
            AddCondition(new PlayerTargetCrouchIdleCondition(MyManager, "CrouchIdle"));
            AddCondition(new PlayerTargetIdleCondition(MyManager, "Idle"));
            AddCondition(new PlayerTargetDeadCondition(MyManager, "Dead"));
        }
        
        public override void OnEnterState()
        {
            MyManager.moveSpeed = 1.6f;
        }
    }
}