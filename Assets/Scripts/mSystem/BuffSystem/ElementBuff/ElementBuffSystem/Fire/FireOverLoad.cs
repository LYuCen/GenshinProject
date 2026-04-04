using mSystem.HitTextSystem;
using mSystem.ScopeHitSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary> 元素反应类型：火超载 (剧变) </summary>
    public class FireOverLoad : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            if (targetGauges[4] > 0) // 遇到雷
            {
                targetGauges[4] = 0;
                attackGauge = 0;

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 4, Text = "超载", textType = 2
                });

                ScopeHitSystem.ScopeHitSystem.Instance.EnQueue(new ScopeHitComponent {
                    Transform = target, Element = 4, ElementMeasure = 0, Harm = 20,
                    Pos = target.position + Vector3.up * 0.5f, Radius = 2f, IsHurtSelf = true
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 1f, BonusDamage = 20f };
            }
            return ReactionResult.None;
        }
    }
}