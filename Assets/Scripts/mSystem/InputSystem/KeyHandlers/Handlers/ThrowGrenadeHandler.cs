using System.Collections.Generic;
using mSystem.InputSystem.mEvent;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class ThrowGrenadeHandler : KeyHandler, ICanSendEvent
    {
        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var s in states)
            {
                if (s.GetIsDown() && s.GetKeyName() == "ThrowGrenade")
                {
                    this.SendEvent(new GetGrenade());
                }
            }
        }
        
        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}