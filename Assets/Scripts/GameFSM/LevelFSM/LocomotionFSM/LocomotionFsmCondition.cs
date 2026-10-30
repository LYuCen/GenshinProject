using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM
{
    /// <summary>
    /// 运动状态机判定 (Locomotion)
    /// </summary>
    public class LocomotionFsmCondition : AbstractCondition<Locomotion>
    {
        public LocomotionFsmCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Locomotion";
            return true;
        }
    }
}
