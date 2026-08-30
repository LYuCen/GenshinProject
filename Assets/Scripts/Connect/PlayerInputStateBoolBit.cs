namespace Connect
{
    public static class PlayerInputStateBoolBit
    {
        public const ushort BUTTON_JUMP   = 1 << 0;
        public const ushort BUTTON_CROUCH = 1 << 1;
        public const ushort BUTTON_SPRINT = 1 << 2;
        public const ushort BUTTON_ADS = 1 << 4;
        public const ushort BUTTON_RELOAD = 1 << 5;
        public const ushort BUTTON_SHOOT = 1 << 6;
    }
}