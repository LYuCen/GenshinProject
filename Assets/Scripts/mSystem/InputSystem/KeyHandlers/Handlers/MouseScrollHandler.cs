using System.Collections.Generic;
using mSystem.InputSystem.mEvent;
using mSystem.InputSystem.mKey;
using QFramework;
using UnityEngine;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class MouseScrollHandler : KeyHandler, ICanSendEvent
    {
        private float _nextTime;
        private MouseScrollEvent _event = new ();
        private ICanSendEvent _canSendEventImplementation;

        public override void Handle(List<KeyState> states)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Time.time > _nextTime && scroll != 0)
            {
                _nextTime = Time.time + 0.3f;
                _event.Scroll = scroll;
                this.SendEvent(_event);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}