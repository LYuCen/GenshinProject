using ShootingTarget;

namespace GameFSM.FSM.PlayerTargetFSM.State
{
    public class PlayerTargetDeadState : AbstractState<PlayerTarget>
    {
        public PlayerTargetDeadState(PlayerTarget manager) : base(manager)
        {
        }
        
        public override void OnEnterState()
        {
            MyManager.moveSpeed = 0;
            MyManager.CreateDeathSnapshot();
            MyManager.gameObject.SetActive(false);
            MyManager.Dead?.Invoke(MyManager);
        }
    }
}