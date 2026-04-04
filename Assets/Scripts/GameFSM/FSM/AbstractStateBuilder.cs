using System.Collections.Generic;

namespace GameFSM.FSM
{
    /// <summary>
    /// 状态构建器，用于为状态机创建状态字典
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractStateBuilder<T>
    {
        // 状态列表是否构建完成
        public bool IsBuilt { get; protected set; }
        protected T MyManager;

        public AbstractStateBuilder(T manager)
        {
            IsBuilt = false;
            MyManager = manager;
        }

        // 返回创建出状态的字典
        public abstract Dictionary<string, AbstractState<T>> CreateStates();
    }
}