using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class ChangeWeaponHandler : KeyHandler, ICanSendEvent
    {

        public ChangeWeaponHandler(InputContext context) : base(context)
        {
        }

        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var key in states)
            {
                if (key.GetKeyName() == "MainGun")
                {
                    Context.GetPrimaryWeapon.Down = key.GetIsDown();
                }
                else if (key.GetKeyName() == "MainGun2")
                {
                    Context.GetSecondaryWeapon.Down = key.GetIsDown();
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}