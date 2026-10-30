using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InAir
{
    /// <summary>
    /// 移动跳跃状态判定 (Jump)
    /// </summary>
    public class MoveJumpStateCondition : AbstractCondition<Locomotion>
    {
        public MoveJumpStateCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Jump";
            return MyManager.IsJump;
        }
    }
}
