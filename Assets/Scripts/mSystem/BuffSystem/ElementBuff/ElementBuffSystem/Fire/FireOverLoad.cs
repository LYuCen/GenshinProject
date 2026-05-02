using mSystem.HitTextSystem;
using mSystem.ScopeHitSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary>
    /// (火)超载反应
    /// </summary>
    public class FireOverLoad : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[4].Enable) // 遇到雷
            {
                float val = controller.GetElement(4);
                controller.ConsumeElement(4, attackGauge);
                attackGauge = Mathf.Max(0, attackGauge - val);

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