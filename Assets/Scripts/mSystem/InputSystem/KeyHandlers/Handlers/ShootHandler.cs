using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class ShootHandler : KeyHandler, ICanSendEvent
    {
        public ShootHandler(InputContext context) : base(context)
        {
        }

        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var key in states)
            {
                if (key.GetKeyName() == "InShoot")
                {
                    Context.SecondaryAction.Down = key.GetIsDown();
                    Context.SecondaryAction.Press = key.GetIsLongPress();
                    Context.SecondaryAction.Up = key.GetIsUp();
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}