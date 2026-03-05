using UnityEngine;

namespace mSystem.EQSSystem.EqsContext
{
    /// <summary>
    /// 环境信息接口
    /// </summary>
    public interface IEqsContext
    {
        Transform Querier { get; }
        Transform Target { get; }
    }
}