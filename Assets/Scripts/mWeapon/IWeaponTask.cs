namespace mWeapon
{
    /// <summary>
    /// 武器状态行为生命周期
    /// </summary>
    public interface IWeaponTask
    {
        /// <summary>
        /// 状态开始
        /// </summary>
        void Start();

        /// <summary>
        /// 状态运行
        /// </summary>
        void Tick();

        /// <summary>
        /// 状态打断
        /// </summary>
        void Cancel();

        /// <summary>
        /// 是否完成
        /// </summary>
        bool Finished { get; }
    }
}