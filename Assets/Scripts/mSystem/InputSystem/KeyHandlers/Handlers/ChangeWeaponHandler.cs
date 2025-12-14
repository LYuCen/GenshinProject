using System.Collections.Generic;
using mSystem.InputSystem.mEvent;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class ChangeWeaponHandler : KeyHandler, ICanSendEvent
    {
        private ChangeGunEvent _event = new ChangeGunEvent();
        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var key in states)
            {
                if (!key.GetIsDown()) continue;

                if (key.GetKeyName() == "MainGun")
                {
                    _event.GunIndex = 0;
                    this.SendEvent(_event);
                }
                else if (key.GetKeyName() == "MainGun2")
                {
                    _event.GunIndex = 1;
                    this.SendEvent(_event);
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}