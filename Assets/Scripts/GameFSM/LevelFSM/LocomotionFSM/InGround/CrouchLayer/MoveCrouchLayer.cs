using GameFSM.FSM;
using GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer
{
    /// <summary>
    /// 移动蹲姿状态层
    /// </summary>
    public class MoveCrouchLayer : AbstractMixLevelState<Locomotion>
    {
        public MoveCrouchLayer(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveStandLayerCondition(MyManager, "Stand"));
            AddCondition(new MoveIdleStateCondition(MyManager, "Idle"));
        }

        protected override void BuildState()
        {
            States.Add("Walk", new MoveCrouchWalkState(MyManager, new MoveCrouchWalkStateCondition(MyManager, "Walk")));
            States.Add("Sprint", new MoveCrouchSprintState(MyManager, new MoveCrouchSprintStateCondition(MyManager, "Sprint")));
            IsBuilt = true;
        }

        public override void OnUpdateState()
        {
            if (MyManager.JumpDown)
            {
                MyManager.IsCrouch = false;
                MyManager.JumpDown = false;
            }
        }
    }
}
