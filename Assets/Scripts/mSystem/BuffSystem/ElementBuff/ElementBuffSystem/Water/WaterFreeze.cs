using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary> 元素反应类型：水冻结 (控制) </summary>
    public class WaterFreeze : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            // 检测目标是否有冰 (索引 6)
            if (targetGauges[6] > 0)
            {
                // 冻结逻辑：消耗元素量进行冻结
                float consumed = Mathf.Min(attackGauge, targetGauges[6]);
                targetGauges[6] -= consumed;
                attackGauge -= consumed;
                
                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent()
                {
                    transform = target, element = 2, Text = "冻结", textType = 2,
                });

                return new ReactionResult
                {
                    IsReaction = true,
                    DamageMultiplier = 1f,
                    BonusDamage = 0,
                    // 如果需要控制敌人无法行动，可以定义并返回 FreezeEffect
                    // TriggeredEffect = new FreezeEffect(duration: 2f) 
                };
            }

            return ReactionResult.None;
        }
    }
}