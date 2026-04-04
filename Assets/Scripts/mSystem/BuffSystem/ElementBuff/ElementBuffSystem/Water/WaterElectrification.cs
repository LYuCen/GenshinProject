using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HitTextSystem;
using QFramework;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary> 元素反应类型：水感电 (持续) </summary>
    public class WaterElectrification : Reaction, ICanSendEvent
    {
        public override ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges)
        {
            if (targetGauges[4] > 0) // 遇到雷
            {
                targetGauges[2] += attackGauge * 0.8f;
                
                HitTextSystem.HitTextSystem.Instance.EnQueue(new HitTextComponent {
                    transform = target, element = 4, Text = "感电", textType = 2
                });
                
                // 这里可以通过返回 Effect 来接管 Controller 里的旧逻辑
                return new ReactionResult { 
                    IsReaction = true, 
                    DamageMultiplier = 1f,
                    TriggeredEffect = new ElectrifyEffect() 
                };
            }
            return ReactionResult.None;
        }

        public IArchitecture GetArchitecture() => GenshinArchitecture.Interface;
    }
}