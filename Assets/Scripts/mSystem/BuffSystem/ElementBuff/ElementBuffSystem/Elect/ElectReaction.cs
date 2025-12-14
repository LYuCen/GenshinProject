using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect
{
    public class ElectReaction : ElementReaction
    {
        public override float Reaction(Transform transform, float harm, ref float element, float[] haveElement, out bool isReaction)
        {
            isReaction = false;
            for (int i = 0; i < Reactions.Count && element > 0; i++)
            {
                float mp = Reactions[i].MultiplyingPower(ref element, haveElement, transform);
                if (mp > 0) // 触发反应
                {
                    isReaction = true;
                    return harm * mp;
                }
            }

            return harm;
        }
    }
}