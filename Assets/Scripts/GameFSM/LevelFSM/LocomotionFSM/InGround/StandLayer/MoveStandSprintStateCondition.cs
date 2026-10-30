using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer
{
    /// <summary>
    /// 移动站立冲刺判定 (Sprint)
    /// </summary>
    public class MoveStandSprintStateCondition : AbstractCondition<Locomotion>
    {
        public MoveStandSprintStateCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Sprint";
            return MyManager.IsMoveForward && (MyManager.SprintDown || MyManager.IsSprint) && !MyManager.IsAiming;
        }
    }
}
