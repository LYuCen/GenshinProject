using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer
{
    /// <summary>
    /// 玩家站立冲刺状态
    /// </summary>
    public class PlayerStandSprintState : AbstractLevelState<PlayerManager>
    {
        public PlayerStandSprintState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerStandWalkStateCondition(MyManager, "Walk"));
        }
        
        public override void OnEnterState()
        {
            
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.IkModule.SetAimIk(MyManager.GetAimIkTarget());
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
            MyManager.IkModule.SetAimIk(MyManager.GetAimIkTarget());
        }
    }
}