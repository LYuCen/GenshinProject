using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class GetPrimaryWeaponActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.GetPrimaryWeapon;
        protected override void OnDown()
        {
            MyManager.CombatModule.GetPrimaryWeapon();
            MyManager.ViewModule.GetPrimaryWeapon();
        }
    }
}