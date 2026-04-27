using UnityEngine;

namespace mSystem.InputSystem.mKey
{
    /// <summary>
    /// 虚拟按键状态
    /// </summary>
    public abstract class KeyState
    {
        public KeyCode Key; // 按键
        private float _prevClick;   // 上一次点击
        private float _prevDown = -1;    // 上一次按下
        private bool _isDown;
        private bool _isPress;
        private bool _isUp;
        private bool _isLongPress;
        

        public float LongPressInterval = 0.2f; // 长按判定间隔
        public float DoubleClickInterval = 0.3f; // 双击时间间隔

        public void UpdateState()
        {
            // 每帧开始先重置瞬态状态
            _isDown = false;
            _isUp = false;

            if (Input.GetKeyDown(Key))
            {
                KeyDown();
                _prevDown = Time.time;
                _isDown = true;
            }

            if (Input.GetKey(Key))
            {
                KeyPress();
                _isPress = true;
                // 长按
                if (Time.time - _prevDown >= LongPressInterval)
                {
                    _isLongPress = true;
                    KeyLongPress();
                }
            }
            else
            {
                _isPress = false;
                _isLongPress = false;
            }

            if (Input.GetKeyUp(Key))
            {
                KeyUp();
                _isUp = true;
                if (_prevClick > 0 && Time.time - _prevClick < DoubleClickInterval)
                {
                    // 双击
                    KeyDoubleClick();
                    _prevClick = -1;
                }
                else _prevClick = Time.time;
            }
        }


        /// <summary>
        /// 虚拟键按下
        /// </summary>
        protected abstract void KeyDown();

        /// <summary>
        /// 虚拟键按住
        /// </summary>
        protected abstract void KeyPress();

        /// <summary>
        /// 虚拟键长按
        /// </summary>
        protected abstract void KeyLongPress();

        /// <summary>
        /// 虚拟键抬起
        /// </summary>
        protected abstract void KeyUp();

        /// <summary>
        /// 虚拟键双击
        /// </summary>
        protected abstract void KeyDoubleClick();

        /// <summary>
        /// 虚拟键名称
        /// </summary>
        /// <returns>编号字符串</returns>
        public abstract string GetKeyName();
        
        /// <summary>
        /// 虚拟键是否按下
        /// </summary>
        /// <returns>返回是否按下</returns>
        public bool GetIsDown()
        {
            return _isDown;
        }
        
        /// <summary>
        /// 虚拟键是否长按
        /// </summary>
        /// <returns></returns>
        public bool GetIsPress()
        {
            return _isPress;
        }

        /// <summary>
        /// 虚拟键是否抬起
        /// </summary>
        /// <returns></returns>
        public bool GetIsUp()
        {
            return _isUp;
        }

        public bool GetIsLongPress()
        {
            return _isLongPress;
        }
    }
}