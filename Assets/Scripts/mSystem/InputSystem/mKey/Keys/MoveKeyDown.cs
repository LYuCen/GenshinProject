using UnityEngine;

namespace mSystem.InputSystem.mKey.Keys
{
    public class MoveKeyDown : KeyState
    {
        public MoveKeyDown(KeyCode key)
        {
            Key = key;
        }
        
        protected override void KeyDown()
        {
            
        }

        protected override void KeyPress()
        {
            
        }

        protected override void KeyLongPress()
        {
            
        }

        protected override void KeyUp()
        {
            
        }

        protected override void KeyDoubleClick()
        {
            
        }

        public override string GetKeyName()
        {
            return "MoveDown";
        }
    }
}