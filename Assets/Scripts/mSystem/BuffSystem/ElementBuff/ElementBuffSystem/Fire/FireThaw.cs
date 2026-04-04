using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary> 元素反应类型：火融化 (增幅) </summary>
    public class FireThaw : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            // 强火(1) 遇到 冰(6)
            if (targetGauges[6] > 0)
            {
                attackGauge *= 2; // 火打冰是强反应
                if (attackGauge > targetGauges[6])
                {
                    attackGauge -= targetGauges[6];
                    targetGauges[6] = 0;
                }
                else
                {
                    targetGauges[6] -= attackGauge;
                    attackGauge = 0;
                }
                attackGauge /= 2;

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 1, Text = "融化", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 2.0f };
            }
            return ReactionResult.None;
        }
    }
}