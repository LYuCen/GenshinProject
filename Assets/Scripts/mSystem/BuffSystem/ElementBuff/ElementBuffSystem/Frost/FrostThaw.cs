using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    /// <summary>
    /// (冰)融化反应
    /// </summary>
    public class FrostThaw : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[1].Enable)
            {
                float val = controller.GetElement(1);
                controller.ConsumeElement(1, attackGauge / 2f);
                attackGauge = Mathf.Max(0, attackGauge - val * 2f);

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 6, Text = "融化", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 1.5f };
            }
            return ReactionResult.None;
        }
    }
}