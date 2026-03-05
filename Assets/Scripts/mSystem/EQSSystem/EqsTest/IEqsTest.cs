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
        bool IsOpposite { get; }

        void Execute(List<EqsPoint> points, IEqsContext context);
    }
}