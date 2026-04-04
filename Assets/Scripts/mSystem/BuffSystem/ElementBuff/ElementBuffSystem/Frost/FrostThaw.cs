using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    /// <summary> 元素反应类型：冰融化 (增幅) </summary>
    public class FrostThaw : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            if (targetGauges[1] > 0) // 弱冰打火
            {
                attackGauge /= 2;
                if (attackGauge > targetGauges[1]) { attackGauge -= targetGauges[1]; targetGauges[1] = 0; }
                else { targetGauges[1] -= attackGauge; attackGauge = 0; }
                attackGauge *= 2;

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 6, Text = "融化", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 1.5f };
            }
            return ReactionResult.None;
        }
    }
}