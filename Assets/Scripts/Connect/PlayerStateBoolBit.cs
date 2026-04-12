namespace Connect
{
    public static class PlayerStateBoolBit
    {
        public const int BIT_GROUND  = 1 << 0;
        public const int BIT_JUMP    = 1 << 1;
        public const int BIT_SHOOT   = 1 << 2;
        public const int BIT_RELOAD  = 1 << 3;
        public const int BIT_SPRINT  = 1 << 4;
        public const int BIT_THROW   = 1 << 5;
        public const int BIT_CROUCH  = 1 << 6;
        public const int BIT_SLIDING = 1 << 7;
    }
}