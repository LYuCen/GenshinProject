using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 角色上半身空状态
    /// </summary>
    public class PlayerEmptyState : AbstractLeafLevelState<PlayerManager>
    {
        public PlayerEmptyState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerReloadStateCondition(MyManager, "Reload"));
            AddCondition(new PlayerAimStateCondition(MyManager, "Aim"));
            AddCondition(new PlayerThrowStateCondition(MyManager, "Throw"));
            AddCondition(new PlayerBoltStateCondition(MyManager, "Bolt"));
        }

        public override void OnEnterState()
        {
        }
    }
}