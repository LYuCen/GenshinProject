using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class GetSecondaryWeaponActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.GetSecondaryWeapon;
        protected override void OnDown()
        {
            MyManager.CombatModule.GetSecondaryWeapon();
            MyManager.ViewModule.GetSecondaryWeapon();
        }
    }
}