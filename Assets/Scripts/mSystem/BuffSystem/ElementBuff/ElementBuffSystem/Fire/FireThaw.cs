using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary>
    /// (火)融化反应
    /// </summary>
    public class FireThaw : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[6].Enable) // 遇到冰
            {
                float val = controller.GetElement(6);
                controller.ConsumeElement(6, attackGauge * 2f);
                attackGauge = Mathf.Max(0, attackGauge - val / 2f);

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 1, Text = "融化", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 2.0f };
            }
            return ReactionResult.None;
        }
    }
}