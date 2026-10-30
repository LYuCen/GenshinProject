using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer
{
    /// <summary>
    /// 移动蹲姿行走判定 (Walk)
    /// </summary>
    public class MoveCrouchWalkStateCondition : AbstractCondition<Locomotion>
    {
        public MoveCrouchWalkStateCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            return !(MyManager.IsSprint || MyManager.SprintDown);
        }
    }
}
