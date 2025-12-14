using System.Collections.Generic;
using mSystem.InputSystem.mEvent;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class JumpHandler : KeyHandler, ICanSendEvent
    {
        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var key in states)
            {
                if (!key.GetIsDown()) continue;
                
                if(key.GetKeyName() == "Jump") this.SendEvent<JumpEvent>();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}