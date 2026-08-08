using Connect;
using mSystem.InputSystem;
using mSystem.InputSystem.Context;
using Player.ManagerLogic;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class SprintActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.Sprint;
        
        protected override void OnDown()
        {
            if (MyManager.GetLogic() is LocalPlayerLogic logic)
            {
                logic.buttons |= PlayerInputStateBoolBit.BUTTON_SPRINT;
            }
        }
    }
}