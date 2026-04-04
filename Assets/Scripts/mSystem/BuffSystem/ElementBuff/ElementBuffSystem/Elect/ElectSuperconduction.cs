using mSystem.HitTextSystem;
using mSystem.ScopeHitSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect
{
    /// <summary> 元素反应类型：雷超导 (剧变) </summary>
    public class ElectSuperconduction : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            if (targetGauges[6] > 0) // 遇到冰
            {
                targetGauges[6] = 0;
                attackGauge = 0;

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 4, Text = "超导", textType = 2
                });

                ScopeHitSystem.ScopeHitSystem.Instance.EnQueue(new ScopeHitComponent {
                    Transform = target, Element = 6, ElementMeasure = 0, Harm = 10,
                    Pos = target.position, Radius = 2f, IsHurtSelf = true
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 1f, BonusDamage = 10f };
            }
            return ReactionResult.None;
        }
    }
}