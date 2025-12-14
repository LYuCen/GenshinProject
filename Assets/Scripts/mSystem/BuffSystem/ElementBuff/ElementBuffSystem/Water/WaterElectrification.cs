using mSystem.BuffSystem.ElementBuff.Event;
using mSystem.HitTextSystem;
using QFramework;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Water
{
    /// <summary>
    /// 水感电
    /// </summary>
    public class WaterElectrification : Reaction, ICanSendEvent
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
            if (haveElement[4] != 0)
            {
                haveElement[2] += element * 0.8f;
                var component = new HitTextComponent()
                {
                    transform = transform,
                    element = 4,
                    Text = "感电",
                    textType = 2,
                };
                HitTextSystem.HitTextSystem.Instance.EnQueue(component);
                this.SendEvent(new ElectrificationReaction(transform));
                return -1;
            }

            return -1;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}