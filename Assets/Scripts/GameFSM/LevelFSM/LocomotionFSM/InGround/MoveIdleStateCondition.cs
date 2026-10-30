using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround
{
    /// <summary>
    /// 移动待机判定 (Idle)
    /// </summary>
    public class MoveIdleStateCondition : AbstractCondition<Locomotion>
    {
        public MoveIdleStateCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Idle";
            return !MyManager.IsInputMove;
        }
    }
}
