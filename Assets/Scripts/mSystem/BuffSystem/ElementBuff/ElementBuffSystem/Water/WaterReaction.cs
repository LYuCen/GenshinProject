using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    public class WaterReaction : ElementReaction
    {
        public WaterReaction()
        {
            Reactions.Add(new WaterEvaporation());
            Reactions.Add(new WaterFreeze());
            Reactions.Add(new WaterElectrification());
        }
    }
}