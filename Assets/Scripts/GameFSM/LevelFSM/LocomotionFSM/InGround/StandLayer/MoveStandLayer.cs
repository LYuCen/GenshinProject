using GameFSM.FSM;
using GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer
{
    /// <summary>
    /// 移动站立状态层
    /// </summary>
    public class MoveStandLayer : AbstractMixLevelState<Locomotion>
    {
        public MoveStandLayer(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveCrouchLayerCondition(MyManager, "Crouch"));
            AddCondition(new MoveIdleStateCondition(MyManager, "Idle"));
        }

        protected override void BuildState()
        {
            States.Add("Walk", new MoveStandWalkState(MyManager, new MoveStandWalkStateCondition(MyManager, "Walk")));
            States.Add("Sprint", new MoveStandSprintState(MyManager, new MoveStandSprintStateCondition(MyManager, "Sprint")));
            IsBuilt = true;
        }

        public override void OnUpdateState()
        {
            if (MyManager.JumpDown)
            {
                MyManager.IsJump = true;
                MyManager.JumpDown = false;
                MyManager.VerticalSpeed = 4f;
            }
        }
    }
}
