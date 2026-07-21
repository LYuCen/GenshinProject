using GameFSM.FSM.PlayerTargetFSM.PlayerTargetFSMCondition;
using ShootingTarget;

namespace GameFSM.FSM.PlayerTargetFSM.State
{
    public class PlayerTargetCrouchIdleState : AbstractState<PlayerTarget>
    {
        public PlayerTargetCrouchIdleState(PlayerTarget manager) : base(manager)
        {
            AddCondition(new PlayerTargetCrouchWalkCondition(MyManager, "CrouchWalk"));
            AddCondition(new PlayerTargetIdleCondition(MyManager, "Idle"));
            AddCondition(new PlayerTargetWalkCondition(MyManager, "Walk"));
            AddCondition(new PlayerTargetDeadCondition(MyManager, "Dead"));
        }
        
        public override void OnEnterState()
        {
            MyManager.moveSpeed = 0;
        }
    }
}