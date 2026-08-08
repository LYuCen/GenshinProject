using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer
{
    /// <summary>
    /// 玩家蹲姿冲刺状态
    /// </summary>
    public class PlayerCrouchSprintState : AbstractLevelState<PlayerManager>
    {
        public PlayerCrouchSprintState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerCrouchWalkStateCondition(MyManager, "Walk"));
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