using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InAir
{
    /// <summary>
    /// 移动滞空层判定 (Air)
    /// </summary>
    public class MoveAirLayerCondition : AbstractCondition<Locomotion>
    {
        public MoveAirLayerCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Air";
            return !MyManager.IsGround;
        }
    }
}
