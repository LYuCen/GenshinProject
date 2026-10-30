using GameFSM.FSM;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer
{
    /// <summary>
    /// 移动蹲姿状态判定 (Crouch)
    /// </summary>
    public class MoveCrouchLayerCondition : AbstractCondition<Locomotion>
    {
        public MoveCrouchLayerCondition(Locomotion manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Crouch";
            return (MyManager.IsCrouch || MyManager.CrouchDown) && MyManager.IsInputMove;
        }
    }
}
