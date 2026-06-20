using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家换弹状态
    /// </summary>
    public class PlayerReloadState : AbstractLevelState<PlayerManager>
    {
        public PlayerReloadState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerAimStateCondition(MyManager,  "Aim"));
            AddCondition(new PlayerThrowStateCondition(MyManager, "Throw"));
            AddCondition(new PlayerEmptyStateCondition(MyManager, "Empty"));
        }
        
        public override void OnEnterState()
        {
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.IkModule.SetAimIk(0);
            MyManager.IkModule.SetHandIk(0);
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
            
        }
    }
}