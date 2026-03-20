using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;

namespace mSystem.EQSSystem.EqsTest
{
    /// <summary>
    /// 候选点评分
    /// </summary>
    public interface IEqsTest
    {
        float Weight { get; }
        
        /// <summary>
        /// 是否取反
        /// </summary>
        bool IsOpposite { get; set; }

        void Execute(List<EqsPoint> points, IEqsContext context);
    }
}