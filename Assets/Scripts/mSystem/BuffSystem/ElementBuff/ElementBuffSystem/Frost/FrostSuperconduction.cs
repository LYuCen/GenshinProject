using mSystem.HitTextSystem;
using mSystem.ScopeHitSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    /// <summary>
    /// (冰)超导反应
    /// </summary>
    public class FrostSuperconduction : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[4].Enable)
            {
                float val = controller.GetElement(4);
                controller.ConsumeElement(4, attackGauge);
                attackGauge = Mathf.Max(0, attackGauge - val);

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 4, Text = "超导", textType = 2
                });

                ScopeHitSystem.ScopeHitSystem.Instance.EnQueue(new ScopeHitComponent {
                    Transform = target, Element = 6, ElementMeasure = 0, Harm = 20,
                    Pos = target.position + Vector3.up * 0.5f, Radius = 2f, IsHurtSelf = true
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 1f, BonusDamage = 20f };
            }
            return ReactionResult.None;
        }
    }
}