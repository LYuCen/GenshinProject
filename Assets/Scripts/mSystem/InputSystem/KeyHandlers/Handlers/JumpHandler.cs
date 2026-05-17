using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class JumpHandler : KeyHandler, ICanSendEvent
    {
        public JumpHandler(InputContext context) : base(context)
        {
        }

        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var key in states)
            {
                
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}