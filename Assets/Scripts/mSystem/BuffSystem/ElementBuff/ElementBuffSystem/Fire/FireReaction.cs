using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    public class FireReaction : ElementReaction
    {
        // 这是你代码里的那个 List<Reaction> Reactions;

        public override ReactionResult Reaction(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            // 遍历火元素支持的所有反应：蒸发、融化、超载等
            for (int i = 0; i < Reactions.Count; i++)
            {
                // 调用具体子反应的 Process 方法
                // 注意：这里传了 ref attackGauge，子反应内部会消耗元素量
                var result = Reactions[i].Process(target, baseHarm, ref attackGauge, targetGauges);

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