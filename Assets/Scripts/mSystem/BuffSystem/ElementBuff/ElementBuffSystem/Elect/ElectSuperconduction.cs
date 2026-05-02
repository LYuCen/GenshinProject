using mSystem.HitTextSystem;
using mSystem.ScopeHitSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect
{
    /// <summary>
    /// (雷)超导反应
    /// </summary>
    public class ElectSuperconduction : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[6].Enable) // 遇到冰
            {
                float val = controller.GetElement(1);
                controller.ConsumeElement(6, attackGauge);
                attackGauge = Mathf.Max(0, attackGauge - val);

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