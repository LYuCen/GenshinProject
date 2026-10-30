namespace mInterface
{
    public interface IClientModule
    {
        /// <summary>
        /// 模块初始化
        /// </summary>
        public void Init();

        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="dt">帧间隔</param>
        public void Update(float dt);

        /// <summary>
        /// 移除调用
        /// </summary>
        public void Dispose();
    }
}