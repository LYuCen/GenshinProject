using mSystem.HitTextSystem;
using mSystem.ScopeHitSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    /// <summary>
    /// 火超载
    /// </summary>
    public class FireOverLoad : Reaction
    {
        public override float MultiplyingPower(ref float element, float[] haveElement, Transform transform)
        {
            if (haveElement[4] > 0)
            {
                haveElement[4] = 0;
                element = 0;
                
                
                HitTextComponent component = new HitTextComponent()
                {
                    transform = transform,
                    element = 4,
                    Text = "超载",
                    textType = 2,
                };
                HitTextSystem.HitTextSystem.Instance.EnQueue(component);
                var scopeHitComponent = new ScopeHitComponent()
                {
                    Transform = transform,
                    Element = 4,
                    ElementMeasure = 0,
                    Harm = 20,
                    Pos = transform.position + Vector3.down * -0.5f,
                    Radius = 2f,
                    IsHurtSelf = true
                };
                ScopeHitSystem.ScopeHitSystem.Instance.EnQueue(scopeHitComponent);
                return 1f;
            }

            return -1;
        }
    }
}