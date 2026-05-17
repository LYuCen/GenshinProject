namespace mSystem.InputSystem
{
    public struct ButtonState
    {
        public bool Down;
        public bool Press;
        public bool Up;
        
        /// <summary>
        /// 当前是否活跃
        /// </summary>
        public bool IsActive()
        {
            return Down || Press || Up;
        }

        /// <summary>
        /// 每帧重置瞬态状态
        /// </summary>
        public void ResetFrameState()
        {
            Down = false;
            Up = false;
        }
    }
}