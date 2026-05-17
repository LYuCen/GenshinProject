using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class MouseScrollActionHandler : InputActionHandler
    {
        public override void Handle(InputContext context)
        {
            MyManager.ViewModule.ChangeElement((int)context.MouseScroll);
        }
    }
}