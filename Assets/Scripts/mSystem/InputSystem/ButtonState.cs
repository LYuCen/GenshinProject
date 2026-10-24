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
    }
}