using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer
{
    /// <summary>
    /// 移动站立层判定 (Stand)
    /// </summary>
    public class MoveStandLayerCondition : AbstractCondition<Locomotion>
    {
        public MoveStandLayerCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Stand";
            return !(MyManager.IsCrouch || MyManager.CrouchDown) && MyManager.IsInputMove;
        }
    }
}
