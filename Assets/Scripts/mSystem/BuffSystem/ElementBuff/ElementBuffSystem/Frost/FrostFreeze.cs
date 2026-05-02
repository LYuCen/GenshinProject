using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    /// <summary>
    /// (冰)冻结反应
    /// </summary>
    public class FrostFreeze : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[2].Enable)
            {
                float val = controller.GetElement(2);
                controller.ConsumeElement(2, attackGauge);
                attackGauge = Mathf.Max(0, attackGauge - val);

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 6, Text = "冻结", textType = 2
                });

                return new ReactionResult { IsReaction = true, DamageMultiplier = 1f };
            }
            return ReactionResult.None;
        }
    }
}