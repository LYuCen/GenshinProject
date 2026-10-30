using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InAir
{
    /// <summary>
    /// 移动跳跃状态
    /// </summary>
    public class MoveJumpState : AbstractLevelState<Locomotion>
    {
        public MoveJumpState(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveFallStateCondition(MyManager, "Fall"));
        }

        public override void OnEnterState()
        {
            
        }

        public override void OnExitState()
        {
            MyManager.IsJump = false;
        }
    }
}
