using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem
{
    public interface IElementBuffSystem
    {
        // 依次输入 反应角色，伤害，元素种类，元素量，元素附着，是否反应
        float Reaction(Transform transform, float harm, int elementType, ref float element, float[] haveElement, out bool isReaction);
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

        public float Reaction(Transform transform, float harm, int elementType,ref float element, float[] haveElement, out bool isReaction)
        {
            return _elementReactions[elementType].Reaction(transform, harm,ref element, haveElement, out isReaction);
        }
    }
}