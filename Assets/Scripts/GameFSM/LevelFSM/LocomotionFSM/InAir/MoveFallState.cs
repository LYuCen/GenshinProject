using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InAir
{
    /// <summary>
    /// 移动坠落状态
    /// </summary>
    public class MoveFallState : AbstractLevelState<Locomotion>
    {
        public MoveFallState(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveJumpStateCondition(MyManager, "Jump"));
        }
    }
}
