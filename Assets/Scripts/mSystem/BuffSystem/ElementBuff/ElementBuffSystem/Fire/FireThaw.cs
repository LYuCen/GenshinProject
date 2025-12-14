using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary>
    /// 火融化
    /// </summary>
    public class FireThaw : Reaction
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
            element *= 2;
            if (haveElement[6] > 0)
            {
                if (element > haveElement[6])
                {
                    element -= haveElement[6];
                    haveElement[6] = 0;
                }
                else
                {
                    haveElement[6] -= element;
                    element = 0;
                }

                element /= 2;
                
                HitTextComponent component = new HitTextComponent()
                {
                    transform = transform,
                    element = 1,
                    Text = "融化",
                    textType = 2,
                };
                HitTextSystem.HitTextSystem.Instance.EnQueue(component);
                return 2f;
            }

            return -1;
        }
    }
}