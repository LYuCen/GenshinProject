using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家上半身状态层
    /// </summary>
    public class UpperBodyLayer : AbstractMixLevelState<PlayerManager>
    {
        public UpperBodyLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
        }

        protected override void BuildState()
        {
            States.Add("Aim", new PlayerAimState(MyManager, new PlayerAimStateCondition(MyManager, "Aim")));
            States.Add("Reload", new PlayerReloadState(MyManager, new PlayerReloadStateCondition(MyManager,  "Reload")));
            States.Add("Throw", new PlayerThrowState(MyManager, new PlayerThrowStateCondition(MyManager, "Throw")));
            States.Add("Empty", new PlayerEmptyState(MyManager, new PlayerEmptyStateCondition(MyManager, "Empty")));
            IsBuilt = true;
        }
    }
}