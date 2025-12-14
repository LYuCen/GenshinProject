using System.Collections.Generic;
using mSystem.InputSystem.mEvent;
using mSystem.InputSystem.mKey;
using QFramework;
using UnityEngine;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class RunHandler : KeyHandler, ICanSendEvent
    {
        private RunEvent _runEvent = new RunEvent();
        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;
            _runEvent.IsRun = 1;
            foreach (var s in states)
            {
                if(s.GetIsDown() && s.GetKeyName() == "Run") this.SendEvent(_runEvent);
            }
        }
        
        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}