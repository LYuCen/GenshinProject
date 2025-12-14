using UnityEngine;

namespace mSystem.InputSystem.mKey.Keys
{
    public class MainGunKey : KeyState
    {
        public MainGunKey(KeyCode keyCode)
        {
            Key = keyCode;
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
            return "MainGun";
        }
    }
}