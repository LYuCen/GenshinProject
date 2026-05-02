using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary>
    /// 水冻结
    /// </summary>
    public class WaterFreeze : Reaction
    {
        public override ReactionResult Process(Transform target, ref float attackGauge, ElementBuffController controller)
        {
            if (controller.Elements[6].Enable)
            {
                float val = controller.GetElement(6);
                controller.ConsumeElement(6, attackGauge);
                attackGauge = Mathf.Max(0, attackGauge - val);
                
                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent()
                {
                    transform = target, element = 2, Text = "冻结", textType = 2,
                });

                return new ReactionResult
                {
                    IsReaction = true,
                    DamageMultiplier = 1f,
                    BonusDamage = 0,
                    // 如果需要控制敌人无法行动，可以取消下面注释
                    // TriggeredEffect = new FreezeEffect() 
                };
            }

            return ReactionResult.None;
        }
    }
}