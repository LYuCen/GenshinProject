using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HitTextSystem;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect
{
    /// <summary>
    /// (雷)感电反应
    /// </summary>
    public class ElectElectrification : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge,  ElementBuffController controller)
        {
            if (controller.Elements[2].Enable)
            {
                HurtComponent c = new HurtComponent()
                {
                    Element = 4,
                    ElementMeasure = attackGauge
                };
                controller.AddElement(c);
                
                // 弹出 UI 文字
                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent()
                {
                    transform = target, element = 4, Text = "感电", textType = 2,
                });
                
                // 返回反应结果：挂载感电持续效果
                return new ReactionResult
                {
                    IsReaction = true,
                    DamageMultiplier = 1f,
                    BonusDamage = 0,
                    TriggeredEffect = new ElectrifyEffect(controller.TimeProvider(), controller) 
                };
            }

            return ReactionResult.None;
        }
    }
}