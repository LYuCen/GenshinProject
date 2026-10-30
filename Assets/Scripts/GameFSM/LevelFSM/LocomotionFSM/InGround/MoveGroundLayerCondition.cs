using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround
{
    /// <summary>
    /// 移动地面层判定 (Ground)
    /// </summary>
    public class MoveGroundLayerCondition : AbstractCondition<Locomotion>
    {
        public MoveGroundLayerCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Ground";
            return MyManager.IsGround;
        }
    }
}
