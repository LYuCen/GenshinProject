using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer
{
    /// <summary>
    /// 移动蹲姿行走状态
    /// </summary>
    public class MoveCrouchWalkState : AbstractLevelState<Locomotion>
    {
        public MoveCrouchWalkState(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveCrouchSprintStateCondition(MyManager, "Sprint"));
        }

        public override void OnEnterState()
        {
            MyManager.LevelSpeed = 1.6f;
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
        }
    }
}
