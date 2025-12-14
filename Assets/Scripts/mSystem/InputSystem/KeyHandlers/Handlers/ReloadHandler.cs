using System.Collections.Generic;
using mSystem.InputSystem.mEvent;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class ReloadHandler : KeyHandler, ICanSendEvent
    {
        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;
            
            foreach (var s in states)
            {
                if (!s.GetIsDown()) continue;
                if(s.GetKeyName() == "Reload") this.SendEvent(new ReloadEvent());
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}