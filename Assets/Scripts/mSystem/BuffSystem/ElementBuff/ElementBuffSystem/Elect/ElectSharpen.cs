using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HitTextSystem;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect
{
    /// <summary>
    /// 雷激化
    /// </summary>
    public class ElectSharpen : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[3].Enable) // 遇到草
            {
                float val = controller.GetElement(3);
                controller.ConsumeElement(3, attackGauge);
                attackGauge = Mathf.Max(0, attackGauge - val);

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 3, Text = "激化", textType = 2
                });
                
                float multiplier = controller.FindEffect<SharpenEffect>() ? 1.25f : 1f;
                
                return new ReactionResult { 
                    IsReaction = true, 
                    DamageMultiplier = multiplier,
                    TriggeredEffect = new SharpenEffect() 
                };
            }
            
            // 发生超激化
            if (controller.FindEffect<SharpenEffect>())
            {
                HurtComponent c = new HurtComponent()
                {
                    Element = 4,
                    ElementMeasure = attackGauge
                };
                controller.AddElement(c);
                
                return new ReactionResult { 
                    IsReaction = true, 
                    DamageMultiplier = 1.25f
                };
            }
            return ReactionResult.None;
        }
    }
}