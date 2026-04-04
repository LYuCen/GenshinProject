using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water;
using mSystem.BuffSystem.ElementBuff.StatusEffect;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem
{
    public struct ReactionResult 
    {
        public float DamageMultiplier; // 增幅倍率
        public float BonusDamage;      // 额外固定伤害
        public IStatusEffect TriggeredEffect; // 持续性效果
        public bool IsReaction;        // 是否触发了反应
        // 提供一个默认静态方法方便返回
        public static ReactionResult None => new ReactionResult { DamageMultiplier = 1f, IsReaction = false };
    }

    
    public interface IElementBuffSystem
    {
        /// <summary>
        /// 返回反应结果
        /// </summary>
        /// <param name="target">击中目标</param>
        /// <param name="baseHarm">基础伤害</param>
        /// <param name="attackElement">元素类型</param>
        /// <param name="attackGauge">元素附着量</param>
        /// <param name="targetGauges">单位元素附着</param>
        /// <returns></returns>
        ReactionResult Reaction(Transform target, float baseHarm, int attackElement, ref float attackGauge, float[] targetGauges);
    }

    public class ElementBuffSystem : IElementBuffSystem
    {
        private ElementReaction[] _elementReactions = new ElementReaction[8];

        public ElementBuffSystem()
        {
            FireReaction fireReaction = new FireReaction(); // 火元素反应
            fireReaction.Reactions.Add(new FireEvaporation());
            fireReaction.Reactions.Add(new FireThaw());
            fireReaction.Reactions.Add(new FireOverLoad());

            WaterReaction waterReaction = new WaterReaction(); // 水元素反应
            waterReaction.Reactions.Add(new WaterEvaporation());
            waterReaction.Reactions.Add(new WaterFreeze());
            waterReaction.Reactions.Add(new WaterElectrification());

            FrostReaction frostReaction = new FrostReaction(); // 冰元素反应
            frostReaction.Reactions.Add(new FrostFreeze());
            frostReaction.Reactions.Add(new FrostThaw());
            frostReaction.Reactions.Add(new FrostSuperconduction());

            ElectReaction electReaction = new ElectReaction(); // 雷元素反应
            electReaction.Reactions.Add(new ElectOverLoad());
            electReaction.Reactions.Add(new ElectSuperconduction());
            electReaction.Reactions.Add(new ElectElectrification());
            
            _elementReactions[1] = fireReaction;
            _elementReactions[2] = waterReaction;
            _elementReactions[4] = electReaction;
            _elementReactions[6] = frostReaction;
        }

        public ReactionResult Reaction(Transform target, float baseHarm, int attackElement, ref float attackGauge, float[] targetGauges)
        {
            return _elementReactions[attackElement].Reaction(target, baseHarm, ref attackGauge, targetGauges);
        }
    }
}