using Common.LevelFSM;
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
        }
        
        public override void OnEnterState()
        {
            MyManager.upperName = "投弹";
            MyManager.bodyIk.enabled = false;
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
            MyManager.upperName = "投弹结束";
            MyManager.bodyIk.enabled = true;
        }
    }
}