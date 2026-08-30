using UnityEngine;

namespace mSystem.InputSystem.mKey.Keys
{
    /// <summary>
    /// 打开战绩面板
    /// </summary>
    public class ScoreboardKey : KeyState
    {
        public ScoreboardKey(KeyCode key)
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
            return "Scoreboard";
        }
    }
}