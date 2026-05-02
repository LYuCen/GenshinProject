using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HitTextSystem;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary>
    /// (水)感电反应
    /// </summary>
    public class WaterElectrification : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge,  ElementBuffController controller)
        {
            if (controller.Elements[4].Enable)
            {
                HurtComponent c = new HurtComponent()
                {
                    Element = 2,
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
                    TriggeredEffect = new ElectrifyEffect() 
                };
            }

            return ReactionResult.None;
        }
    }
}