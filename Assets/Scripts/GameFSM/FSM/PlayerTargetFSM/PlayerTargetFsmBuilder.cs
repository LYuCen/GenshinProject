using System.Collections.Generic;
using GameFSM.FSM.PlayerTargetFSM.State;

namespace GameFSM.FSM.PlayerTargetFSM
{
    public class PlayerTargetFsmBuilder : AbstractStateBuilder<ShootingTarget.PlayerTarget>
    {
        public PlayerTargetFsmBuilder(ShootingTarget.PlayerTarget manager) : base(manager)
        {
        }

        public override Dictionary<string, AbstractState<ShootingTarget.PlayerTarget>> CreateStates()
        {
            var res = new Dictionary<string, AbstractState<ShootingTarget.PlayerTarget>>();
            res.Add("Idle", new PlayerTargetIdleState(MyManager));
            res.Add("CrouchIdle", new PlayerTargetCrouchIdleState(MyManager));
            res.Add("Walk", new PlayerTargetWalkState(MyManager));
            res.Add("CrouchWalk", new PlayerTargetCrouchWalkState(MyManager));
            res.Add("Dead", new PlayerTargetDeadState(MyManager));
            IsBuilt = true;
            return res;
        }
    }
}