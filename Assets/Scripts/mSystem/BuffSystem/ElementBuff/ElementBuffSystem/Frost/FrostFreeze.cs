using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    /// <summary> 元素反应类型：冰冻结 (控制) </summary>
    public class FrostFreeze : Reaction
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            if (targetGauges[2] > 0) // 遇到水
            {
                // 冻结消耗逻辑
                float consumed = Mathf.Min(attackGauge, targetGauges[2]);
                targetGauges[2] -= consumed;
                attackGauge -= consumed;

                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 6, Text = "冻结", textType = 2
                });

                return new ReactionResult { 
                    IsReaction = true, 
                    DamageMultiplier = 1f
                    // TriggeredEffect = new FreezeEffect() // 可挂载冻结状态
                };
            }
            return ReactionResult.None;
        }
    }
}