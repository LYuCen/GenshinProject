using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class AttackHandler : KeyHandler, ICanSendEvent
    {
        public AttackHandler(InputContext context) : base(context)
        {
        }

        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var key in states)
            {
                if (key.GetKeyName() == "MouseLeft")
                {
                    Context.PrimaryAction.Down = key.GetIsDown();
                    Context.PrimaryAction.Press = key.GetIsPress();
                    Context.PrimaryAction.Up = key.GetIsUp();
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}