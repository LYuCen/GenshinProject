using GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition;

namespace GameFSM.FSM.PlayerTargetFSM.State
{
    public class PlayerTargetWalkState : AbstractState<ShootingTarget.PlayerTarget>
    {
        public PlayerTargetWalkState(ShootingTarget.PlayerTarget manager) : base(manager)
        {
            AddCondition(new PlayerTargetIdleCondition(MyManager, "Idle"));
            AddCondition(new PlayerTargetCrouchWalkCondition(MyManager, "CrouchWalk"));
            AddCondition(new PlayerTargetCrouchIdleCondition(MyManager, "CrouchIdle"));
            AddCondition(new PlayerTargetDeadCondition(MyManager, "Dead"));
        }
        
        public override void OnEnterState()
        {
            MyManager.moveSpeed = 1.6f;
        }
    }
}