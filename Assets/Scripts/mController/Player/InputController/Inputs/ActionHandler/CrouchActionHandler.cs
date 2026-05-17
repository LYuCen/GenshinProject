using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class CrouchActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.Crouch;

        protected override void OnDown()
        {
            MyManager.MoveModule.Crouch();
        }
    }
}