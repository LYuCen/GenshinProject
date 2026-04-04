using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HitTextSystem;
using QFramework;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect
{
    /// <summary> 元素反应类型：雷感电 (持续/剧变) </summary>
    public class ElectElectrification : Reaction, ICanSendEvent
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            // 检测目标是否有水 (索引 2)
            if (targetGauges[2] > 0)
            {
                // 感电逻辑：雷水共存，通常会增加附着并触发持续伤害
                targetGauges[4] += attackGauge * 0.8f;
                
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

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}