namespace Common.FSM
{
    /// <summary>
    /// FSM 条件基类
    /// </summary>
    public abstract class AbstractCondition<T>
    {
        public string NextStateName { get; }
        protected T MyManager;

        public AbstractCondition(T manager, string nextStateName)
        {
            MyManager = manager;
            NextStateName = nextStateName;
        }

        /// <summary>
        /// 检查条件, 如果满足返回 true 和下一个状态
        /// </summary>
        /// <param name="nextState"></param>
        /// <returns></returns>
        public abstract bool CheckCondition(out string nextState);
    }
}