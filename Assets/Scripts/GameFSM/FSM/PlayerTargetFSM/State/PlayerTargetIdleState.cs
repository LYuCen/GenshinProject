using GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition;

namespace GameFSM.FSM.PlayerTargetFSM.State
{
    public class PlayerTargetIdleState : AbstractState<ShootingTarget.PlayerTarget>
    {
        public PlayerTargetIdleState(ShootingTarget.PlayerTarget manager) : base(manager)
        {
            AddCondition(new PlayerTargetCrouchIdleCondition(MyManager, "CrouchIdle"));
            AddCondition(new PlayerTargetWalkCondition(MyManager, "Walk"));
            AddCondition(new PlayerTargetCrouchWalkCondition(MyManager, "CrouchWalk"));
            AddCondition(new PlayerTargetDeadCondition(MyManager, "Dead"));
        }

        public override void OnEnterState()
        {
            MyManager.moveSpeed = 0;
        }
    }
}