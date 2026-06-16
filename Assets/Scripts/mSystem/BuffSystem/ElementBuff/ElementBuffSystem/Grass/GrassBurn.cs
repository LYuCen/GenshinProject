using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HitTextSystem;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Grass
{
    /// <summary>
    /// 草燃烧
    /// </summary>
    public class GrassBurn : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[1].Enable) // 遇到火
            {
                controller.Elements[1].Enable = false;

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 1, Text = "燃烧", textType = 2
                });
                
                return new ReactionResult { 
                    IsReaction = true, 
                    DamageMultiplier = 1f,
                    TriggeredEffect = new BurnEffect(controller.TimeProvider(), controller) 
                };
            }
            return ReactionResult.None;
        }
    }
}