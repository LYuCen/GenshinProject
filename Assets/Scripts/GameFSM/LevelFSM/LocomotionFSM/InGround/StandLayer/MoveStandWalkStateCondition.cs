using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer
{
    /// <summary>
    /// 移动站立行走判定 (Walk)
    /// </summary>
    public class MoveStandWalkStateCondition : AbstractCondition<Locomotion>
    {
        public MoveStandWalkStateCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            
            return !(MyManager.IsSprint || MyManager.SprintDown);
        }
    }
}
