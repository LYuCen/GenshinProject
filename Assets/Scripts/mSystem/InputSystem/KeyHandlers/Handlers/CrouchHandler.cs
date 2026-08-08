using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class CrouchHandler : KeyHandler, ICanSendEvent
    {
        public CrouchHandler(InputContext context) : base(context)
        {
        }

        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;
            
            foreach (var key in states)
            {
                if (key.GetKeyName() == "Crouch")
                {
                    Context.Crouch.Down = key.GetIsDown();
                    Context.Crouch.Press = key.GetIsPress();
                    Context.Crouch.Up = key.GetIsUp();
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}