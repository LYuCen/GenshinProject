using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;

namespace mSystem.InputSystem.KeyHandlers
{
    public abstract class KeyHandler
    {
        protected InputContext Context;
        public bool Enable = true;

        public KeyHandler(InputContext context)
        {
            Context = context;
        }

        public abstract void Handle(List<KeyState> states);
    }
}