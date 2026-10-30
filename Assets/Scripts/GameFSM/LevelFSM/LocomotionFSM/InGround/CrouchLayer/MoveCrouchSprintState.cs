using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer
{
    /// <summary>
    /// 移动蹲姿冲刺状态
    /// </summary>
    public class MoveCrouchSprintState : AbstractLevelState<Locomotion>
    {
        public MoveCrouchSprintState(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveCrouchWalkStateCondition(MyManager, "Walk"));
        }

        public override void OnEnterState()
        {
            MyManager.LevelSpeed = 3.5f;
            MyManager.IsSprint = true;
        }

        public override void OnUpdateState()
        {
            if (!MyManager.IsMoveForward) MyManager.IsSprint = false;
            if (MyManager.IsAiming) MyManager.IsSprint = false;
        }

        public override void OnExitState()
        {
            
        }
    }
}
