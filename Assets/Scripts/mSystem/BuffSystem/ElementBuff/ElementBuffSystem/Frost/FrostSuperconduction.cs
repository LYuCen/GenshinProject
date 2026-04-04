using mSystem.HitTextSystem;
using mSystem.ScopeHitSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    /// <summary> 元素反应类型：冰超导 (剧变/减防) </summary>
    public class FrostSuperconduction : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            // 检测目标是否有雷 (索引 4)
            if (targetGauges[4] > 0)
            {
                // 超导逻辑：消耗掉双元素
                targetGauges[4] = 0;
                attackGauge = 0;
                
                // 弹出 UI 文字
                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent()
                {
                    transform = target, element = 4, Text = "超导", textType = 2,
                });

                // 触发范围冰元素伤害
                ScopeHitSystem.ScopeHitSystem.Instance.EnQueue(new ScopeHitComponent()
                {
                    Transform = target,
                    Element = 6,
                    ElementMeasure = 0,
                    Harm = 20,
                    Pos = target.position + Vector3.up * 0.5f,
                    Radius = 2f,
                    IsHurtSelf = true
                });

                return new ReactionResult
                {
                    IsReaction = true,
                    DamageMultiplier = 1f,
                    BonusDamage = 20f, // 剧变反应提供固定伤害值
                    TriggeredEffect = null // 超导通常是瞬发，不挂载持续效果
                };
            }

            return ReactionResult.None;
        }
    }
}