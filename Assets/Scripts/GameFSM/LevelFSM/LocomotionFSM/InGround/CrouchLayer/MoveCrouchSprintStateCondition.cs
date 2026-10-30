using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer
{
    /// <summary>
    /// 移动蹲姿冲刺判定 (Sprint)
    /// </summary>
    public class MoveCrouchSprintStateCondition : AbstractCondition<Locomotion>
    {
        public MoveCrouchSprintStateCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Sprint";
            return MyManager.IsMoveForward && (MyManager.SprintDown || MyManager.IsSprint) && !MyManager.IsAiming;
        }
    }
}
