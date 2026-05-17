using mEnum.Flags;

namespace mEnum.Query
{
    /// <summary>
    /// 角色状态查询
    /// </summary>
    public struct PlayerMoveQuery
    {
        private int _state;
        
        /// <summary>
        /// 检查是否状态是否为 true
        /// </summary>
        /// <param name="flag">状态标签</param>
        /// <returns></returns>
        public bool HasState(PlayerMoveFlags flag)
        {
            return (_state & (int)flag) != 0;
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="flags">状态标签</param>
        public void AddFlags(PlayerMoveFlags flags)
        {
            _state |= (int)flags;
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="flags">状态标签</param>
        public void RemoveFlags(PlayerMoveFlags flags)
        {
            _state &= ~(int)flags;
        }

        public void SetState(int state)
        {
            _state = state;
        }
    }
}