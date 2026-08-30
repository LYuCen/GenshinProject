using System.Collections.Generic;
using mEvent;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class ShowScoreboardHandler : KeyHandler, ICanSendEvent
    {
        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;

            foreach (var k in states)
            {
                if (k.GetIsDown() && k.GetKeyName() == "Scoreboard")
                {
                    this.SendEvent<ShowScoreboardEvent>();
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}