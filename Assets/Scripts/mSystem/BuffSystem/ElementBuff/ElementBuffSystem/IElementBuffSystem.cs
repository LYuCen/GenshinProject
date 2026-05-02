using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Grass;
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
        /// <param name="attackElement">元素类型</param>
        /// <param name="attackGauge">元素附着量</param>
        /// <param name="controller">单位元素附着</param>
        /// <returns></returns>
        ReactionResult Reaction(Transform target, int attackElement, ref float attackGauge, ElementBuffController controller);
    }

    public class ElementBuffSystem : IElementBuffSystem
    {
        private ElementReaction[] _elementReactions = new ElementReaction[8];

        public ElementBuffSystem()
        {
            _elementReactions[1] = new FireReaction(); // 火元素反应
            _elementReactions[2] = new WaterReaction(); // 水元素反应
            _elementReactions[3] = new GrassReaction(); // 草元素反应
            _elementReactions[4] = new ElectReaction(); // 雷元素反应
            _elementReactions[6] = new FrostReaction(); // 冰元素反应
        }

        public ReactionResult Reaction(Transform target, int attackElement, ref float attackGauge,  ElementBuffController controller)
        {
            return _elementReactions[attackElement].Reaction(target, ref attackGauge, controller);
        }
    }
}