namespace mEnum.Flags
{
    public enum WeaponFlags
    {   
        /// <summary>空状态</summary>
        None = 0,
        /// <summary>换弹</summary>
        Reload = 1 << 0,
        /// <summary>拉栓</summary>
        Blot = 1 << 1,
        /// <summary>射击</summary>
        Shoot = 1 << 2,
        /// <summary>投掷</summary>
        Throw = 1 << 3,
        /// <summary>肩射</summary>
        Shoulder = 1 << 4,
    }
}