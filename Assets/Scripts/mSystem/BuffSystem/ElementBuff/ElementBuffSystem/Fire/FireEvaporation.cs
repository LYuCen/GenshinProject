using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary> 元素反应类型：火蒸发 (增幅) </summary>
    public class FireEvaporation : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            // 弱火(1) 遇到 水(2)
            if (targetGauges[2] > 0)
            {
                attackGauge /= 2; // 火打水是弱反应
                if (attackGauge > targetGauges[2])
                {
                    attackGauge -= targetGauges[2];
                    targetGauges[2] = 0;
                }
                else
                {
                    targetGauges[2] -= attackGauge;
                    attackGauge = 0;
                }
                attackGauge *= 2; // 恢复计算量

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 1, Text = "蒸发", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 1.5f };
            }
            return ReactionResult.None;
        }
    }
}