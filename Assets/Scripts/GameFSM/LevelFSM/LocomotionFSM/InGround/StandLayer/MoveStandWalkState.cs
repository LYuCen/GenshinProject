using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer
{
    /// <summary>
    /// 移动站立行走状态
    /// </summary>
    public class MoveStandWalkState : AbstractLevelState<Locomotion>
    {
        public MoveStandWalkState(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveStandSprintStateCondition(MyManager, "Sprint"));
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
