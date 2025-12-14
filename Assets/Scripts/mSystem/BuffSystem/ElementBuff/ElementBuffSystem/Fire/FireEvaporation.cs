using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary>
    /// 火蒸发
    /// </summary>
    public class FireEvaporation : Reaction
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
            element /= 2;
            if (haveElement[2] > 0)
            {
                if (element > haveElement[2])
                {
                    element -= haveElement[2];
                    haveElement[2] = 0;
                }
                else
                {
                    haveElement[2] -= element;
                    element = 0;
                }
                
                element *= 2;
                
                HitTextComponent component = new HitTextComponent()
                {
                    transform = transform,
                    element = 1,
                    Text = "蒸发",
                    textType = 2,
                };
                HitTextSystem.HitTextSystem.Instance.EnQueue(component);
                return 1.5f;
            }

            return -1;
        }
    }
}