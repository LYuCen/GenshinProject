using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer
{
    /// <summary>
    /// 玩家蹲姿行走状态
    /// </summary>
    public class PlayerCrouchWalkState : AbstractLevelState<PlayerManager>
    {
        public PlayerCrouchWalkState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerCrouchSprintStateCondition(MyManager, "Sprint"));
        }
        
        public override void OnEnterState()
        {
            MyManager.speed = 1.6f;
            MyManager.gunCube.SetParent(MyManager.hand);
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
        }
    }
}