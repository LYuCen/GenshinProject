using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem
{
    public abstract class Reaction
    {
        public abstract ReactionResult Process(Transform target, ref float attackGauge,  ElementBuffController controller);
    }
}