using mSystem.BuffSystem.ElementBuff.Event;
using mSystem.HitTextSystem;
using QFramework;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect
{
    public class ElectElectrification : Reaction, ICanSendEvent
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
            if (haveElement[2] != 0)
            {
                haveElement[4] += element * 0.8f;
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