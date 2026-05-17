using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class PrimaryActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.PrimaryAction;

        protected override void OnDown()
        {
            MyManager.MoveModule.Sprint();
        }

        protected override void OnPress()
        {
            MyManager.MoveModule.Sprint();
        }

        protected override void OnUp()
        {
            MyManager.MoveModule.Sprint();
        }
    }
}