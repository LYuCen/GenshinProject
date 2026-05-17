using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class MoveActionHandler : InputActionHandler
    {
        public override void Handle(InputContext context)
        {
            MyManager.MoveModule.InputMove(context.Move);
        }
    }
}