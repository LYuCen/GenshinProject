using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    /// <summary>
    /// 冰融化
    /// </summary>
    public class FrostThaw : Reaction
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
            element /= 2;
            if (haveElement[1] > 0)
            {
                if (element > haveElement[1])
                {
                    element -= haveElement[1];
                    haveElement[1] = 0;
                }
                else
                {
                    haveElement[1] -= element;
                    element = 0;
                }

                HitTextComponent component = new HitTextComponent()
                {
                    transform = transform,
                    element = 6,
                    Text = "融化",
                    textType = 2,
                };
                HitTextSystem.HitTextSystem.Instance.EnQueue(component);
                element *= 2;
                return 1.5f;
            }

            return -1;
        }
    }
}