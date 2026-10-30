using GameFSM.FSM;
using GameFSM.LevelFSM.LocomotionFSM.InGround;

namespace GameFSM.LevelFSM.LocomotionFSM.InAir
{
    /// <summary>
    /// 移动滞空层
    /// </summary>
    public class MoveAirLayer : AbstractMixLevelState<Locomotion>
    {
        public MoveAirLayer(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveGroundLayerCondition(MyManager, "Ground"));
        }

        protected override void BuildState()
        {
            States.Add("Jump", new MoveJumpState(MyManager, new MoveJumpStateCondition(MyManager, "Jump")));
            States.Add("Fall", new MoveFallState(MyManager, new MoveFallStateCondition(MyManager, "Fall")));
            IsBuilt = true;
        }
    }
}
