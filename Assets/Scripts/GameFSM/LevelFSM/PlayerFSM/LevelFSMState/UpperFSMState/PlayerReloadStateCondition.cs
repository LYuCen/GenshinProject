using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家换弹状态判定 (Reload)
    /// </summary>
    public class PlayerReloadStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerReloadStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Reload";
            return MyManager.IsReload;
        }
    }
}