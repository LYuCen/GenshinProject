using GameFSM.FSM;
using GameFSM.LevelFSM.LocomotionFSM.InAir;
using GameFSM.LevelFSM.LocomotionFSM.InGround;

namespace GameFSM.LevelFSM.LocomotionFSM
{
    /// <summary>
    /// 移动状态机
    /// </summary>
    public class LocomotionFsm : AbstractMixLevelState<Locomotion>
    {
        public LocomotionFsm(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
        }

        protected override void BuildState()
        {
            States.Add("Ground", new MoveGroundLayer(MyManager, new MoveGroundLayerCondition(MyManager, "Ground")));
            States.Add("Air", new MoveAirLayer(MyManager, new MoveAirLayerCondition(MyManager, "Air")));
            IsBuilt = true;
        }

        public override void OnEnterState()
        {
        }
    }
}
