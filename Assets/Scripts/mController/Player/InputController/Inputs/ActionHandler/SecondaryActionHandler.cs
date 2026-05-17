using mEnum.Flags;
using mEnum.Query;
using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs.ActionHandler
{
    public class SecondaryActionHandler : ButtonActionHandler
    {
        protected override ButtonState GetButtonState(InputContext context) => context.SecondaryAction;

        protected override void OnDown()
        {
            MyManager.CombatModule.DownSecondaryAction();
            WeaponQuery query = MyManager.CombatModule.GetQuery();
            if (query.HasState(WeaponFlags.Shoot)) MyManager.MoveModule.GetQuery();
        }

        protected override void OnPress()
        {
            MyManager.CombatModule.PressSecondaryAction();
        }

        protected override void OnUp()
        {
            MyManager.CombatModule.UpSecondaryAction();
        }
    }
}