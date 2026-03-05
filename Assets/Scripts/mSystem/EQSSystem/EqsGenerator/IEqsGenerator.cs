using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;

namespace mSystem.EQSSystem.EqsGenerator
{
    /// <summary>
    /// 候选点生成接口
    /// </summary>
    public interface IEqsGenerator
    {
        List<EqsPoint> Generate(IEqsContext context);
    }
}