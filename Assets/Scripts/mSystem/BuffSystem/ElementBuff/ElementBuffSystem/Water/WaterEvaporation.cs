using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary>
    /// (水)蒸发反应
    /// </summary>
    public class WaterEvaporation : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[1].Enable)
            {
                float val = controller.GetElement(1);
                controller.ConsumeElement(1, attackGauge * 2f);
                attackGauge = Mathf.Max(0, attackGauge - val / 2f);

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 2, Text = "蒸发", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 2.0f };
            }
            return ReactionResult.None;
        }
    }
}