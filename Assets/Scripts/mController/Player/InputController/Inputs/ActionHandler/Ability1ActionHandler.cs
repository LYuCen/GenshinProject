using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class Ability1ActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.Ability1;
        protected override void OnDown()
        {
            MyManager.CombatModule.DownAbility1();
        }
    }
}