using UnityEngine;

namespace mSystem.EQSSystem.EqsContext
{
    /// <summary>
    /// 环境信息接口
    /// </summary>
    public interface IEqsContext
    {
        /// <summary>
        /// eqs 判定者
        /// </summary>
        Transform Querier { get; }
        
        /// <summary>
        /// 玩家
        /// </summary>
        Transform Target { get; }
    }
}