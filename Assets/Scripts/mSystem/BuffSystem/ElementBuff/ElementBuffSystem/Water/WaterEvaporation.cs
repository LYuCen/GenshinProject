using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary>
    /// 水蒸发
    /// </summary>
    public class WaterEvaporation : Reaction
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
            element *= 2;
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

                element /= 2;
                HitTextComponent component = new HitTextComponent()
                {
                    transform = transform,
                    element = 2,
                    Text = "蒸发",
                    textType = 2,
                };
                HitTextSystem.HitTextSystem.Instance.EnQueue(component);
                return 2f;
            }

            return -1;
        }
    }
}