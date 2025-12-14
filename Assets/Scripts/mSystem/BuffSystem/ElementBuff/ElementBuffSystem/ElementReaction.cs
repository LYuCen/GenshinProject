using System.Collections.Generic;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem
{
    public abstract class ElementReaction
    {
        public List<Reaction> Reactions = new();

        public abstract float Reaction(Transform transform, float harm,ref float element, float[] haveElement, out bool isReaction);
    }
}