using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HitTextSystem;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary>
    /// 火燃烧
    /// </summary>
    public class FireBurn : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[3].Enable) // 遇到草
            {
                HurtComponent c = new HurtComponent()
                {
                    Element = 1,
                    ElementMeasure = attackGauge
                };
                controller.AddElement(c);

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 1, Text = "燃烧", textType = 2
                });
                
                return new ReactionResult { 
                    IsReaction = true, 
                    DamageMultiplier = 1f,
                    TriggeredEffect = new BurnEffect() 
                };
            }
            return ReactionResult.None;
        }
    }
}