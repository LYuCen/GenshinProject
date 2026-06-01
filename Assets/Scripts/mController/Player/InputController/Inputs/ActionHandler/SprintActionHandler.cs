using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class SprintActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.Sprint;

        protected override void OnDown()
        {
            MyManager.MoveModule.RequestSprint();
        }
    }
}