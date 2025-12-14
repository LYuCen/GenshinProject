using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary>
    /// 水冻结
    /// </summary>
    public class WaterFreeze : Reaction
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
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
                
                HitTextComponent component = new HitTextComponent()
                {
                    transform = transform,
                    element = 2,
                    Text = "冻结",
                    textType = 2,
                };
                HitTextSystem.HitTextSystem.Instance.EnQueue(component);
                return 1f;
            }

            return -1;
        }
    }
}