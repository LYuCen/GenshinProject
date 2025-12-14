using System.Collections.Generic;
using mSystem.InputSystem.mEvent;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class ShootHandler : KeyHandler, ICanSendEvent
    {
        private WeaponSecondaryActionEvent _event = new();
        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var key in states)
            {
                if (key.GetKeyName() == "InShoot")
                {
                    _event.IsDown = key.GetIsDown();
                    _event.IsPress = key.GetIsPress();
                    _event.IsUp = key.GetIsUp();
                    if(_event.IsDown ||  _event.IsPress || _event.IsUp) this.SendEvent(_event);
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}