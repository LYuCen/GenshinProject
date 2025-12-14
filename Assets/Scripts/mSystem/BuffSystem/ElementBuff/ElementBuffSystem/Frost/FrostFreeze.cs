using mSystem.HitTextSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    /// <summary>
    /// 冰冻结
    /// </summary>
    public class FrostFreeze : Reaction
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
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

                HitTextComponent component = new HitTextComponent()
                {
                    transform = transform,
                    element = 6,
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