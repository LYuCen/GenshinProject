using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary>
    /// (火)蒸发反应
    /// </summary>
    public class FireEvaporation : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[2].Enable)
            {
                float val = controller.GetElement(2);
                controller.ConsumeElement(2, attackGauge / 2f);
                attackGauge = Mathf.Max(0, attackGauge - val * 2f);

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 1, Text = "蒸发", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 1.5f };
            }
            return ReactionResult.None;
        }
    }
}