using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class MoveActionHandler : InputActionHandler
    {
        public override void Handle(InputContext context)
        {
            FsmContext.Locomotion.IsInputMove = context.Move.sqrMagnitude >= 0.05f;
            FsmContext.Locomotion.IsMoveForward = context.Move.y >= 0.05f;
            FsmContext.Locomotion.InputMoveVec = context.Move;
        }
    }
}