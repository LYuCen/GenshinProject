using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem
{
    public abstract class Reaction
    {
        public abstract ReactionResult Process(Transform target, float baseHarm, ref float attackGauge, float[] targetGauges);
    }
}