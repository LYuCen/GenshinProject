using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;

namespace mSystem.EQSSystem
{
    /// <summary>
    /// EQS环境查询系统
    /// </summary>
    public interface IEqsSystem
    {
        /// <summary>
        /// 所有候选点
        /// </summary>
        IReadOnlyList<EqsPoint> Points { get; }

        /// <summary>
        /// 处理候选点
        /// </summary>
        /// <param name="context">环境信息</param>
        public void RunQuery(IEqsContext context);

        /// <summary>
        /// 获取最优候选点
        /// </summary>
        /// <returns>候选点信息</returns>
        EqsPoint GetBestPoint();
    }
}