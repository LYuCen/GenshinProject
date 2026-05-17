using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家投掷状态
    /// </summary>
    public class PlayerThrowState : AbstractLevelState<PlayerManager>
    {
        public PlayerThrowState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerReloadStateCondition(MyManager, "Reload"));
            AddCondition(new PlayerAimStateCondition(MyManager, "Aim"));
            AddCondition(new PlayerEmptyStateCondition(MyManager, "Empty"));
        }
        
        public override void OnEnterState()
        {
            
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
            
        }
    }
}