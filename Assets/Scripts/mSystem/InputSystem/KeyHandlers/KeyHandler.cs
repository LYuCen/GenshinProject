using System.Collections.Generic;
using mSystem.InputSystem.mKey;

namespace mSystem.InputSystem.KeyHandlers
{
    public abstract class KeyHandler
    {
        public bool Enable = true;

        public abstract void Handle(List<KeyState> states);
    }
}