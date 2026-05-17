using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class ReloadActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.Reload;

        protected override void OnDown()
        {
        }
    }
}