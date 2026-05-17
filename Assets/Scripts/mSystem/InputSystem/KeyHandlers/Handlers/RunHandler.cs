using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class RunHandler : KeyHandler, ICanSendEvent
    {
        public RunHandler(InputContext context) : base(context)
        {
        }

        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;
            foreach (var s in states)
            {
                if (s.GetKeyName() == "Run")
                {
                    Context.Sprint.Down = s.GetIsDown();
                }
            }
        }
        
        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}