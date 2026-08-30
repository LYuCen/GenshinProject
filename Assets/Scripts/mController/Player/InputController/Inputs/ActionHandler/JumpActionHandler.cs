using Connect;
using mSystem.InputSystem;
using mSystem.InputSystem.Context;
using Player.ManagerLogic;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class JumpActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.Jump;

        protected override void OnDown()
        {
            if (MyManager.GetLogic() is LocalPlayerLogic logic)
            {
                
            }
        }
    }
}