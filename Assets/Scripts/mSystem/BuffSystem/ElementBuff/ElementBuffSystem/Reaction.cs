using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem
{
    public abstract class Reaction
    {
        public abstract float MultiplyingPower(ref float element, float[] haveElement, Transform transform);
    }
}