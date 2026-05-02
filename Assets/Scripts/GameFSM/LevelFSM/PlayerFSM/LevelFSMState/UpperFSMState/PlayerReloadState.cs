using Common.LevelFSM;
using GameFSM.FSM;

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
        }
        
        public override void OnEnterState()
        {
            MyManager.upperName = "换弹";
            MyManager.aimIkWeight = 0;
            MyManager.rHeadIkWeight = 0;
            MyManager.IsShoot = false;
            MyManager.gunCube.SetParent(MyManager.hand);
        }

        public override void OnUpdateState()
        {
            MyManager.IsShoot = false;
        }

        public override void OnExitState()
        {
            MyManager.aimIkWeight = MyManager.IsShoot ? 1 : 0;
            MyManager.rHeadIkWeight = 1;
        }
    }
}