using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary> 元素反应类型：水蒸发 (增幅) </summary>
    public class WaterEvaporation : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            if (targetGauges[1] > 0) // 强水打火
            {
                attackGauge *= 2;
                if (attackGauge > targetGauges[1]) { attackGauge -= targetGauges[1]; targetGauges[1] = 0; }
                else { targetGauges[1] -= attackGauge; attackGauge = 0; }
                attackGauge /= 2;

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 2, Text = "蒸发", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 2.0f };
            }
            return ReactionResult.None;
        }
    }
}