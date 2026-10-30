using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InAir
{
    /// <summary>
    /// 移动坠落判定 (Fall)
    /// </summary>
    public class MoveFallStateCondition : AbstractCondition<Locomotion>
    {
        public MoveFallStateCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Fall";
            return !MyManager.IsJump;
        }
    }
}
