using System.Collections.Generic;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem
{
    public class ElementReaction
    {
        public List<Reaction> Reactions = new();

        public virtual ReactionResult Reaction(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            for (int i = 0; i < Reactions.Count && attackGauge > 0; i++)
            {
                // 调用具体子反应的 Process 方法
                // 注意：这里传了 ref attackGauge，子反应内部会消耗元素量
                var result = Reactions[i].Process(target, ref attackGauge, controller);

                if (result.IsReaction)
                {
                    // 只要触发了其中一种反应，就直接返回该结果
                    return result;
                }
            }

            // 如果遍历完所有可能的反应都没触发（比如目标身上没元素）
            return ReactionResult.None; 
        }
    }
}