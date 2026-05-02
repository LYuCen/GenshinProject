using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    /// <summary>
    /// 燃烧反应效果逻辑
    /// </summary>
    public class BurnEffect : IStatusEffect
    {
        public bool IsFinished { get; private set; }
        private int _val = -1;
        private int _electCd;
        public void OnUpdate(ElementBuffController buff)
        {
            if (_val == -1)
            {
                _val = buff.Elements[3].Val;
                buff.Elements[3].Val = 0;
                buff.Elements[3].Enable = false;
            }
            
            if (_electCd == 0)
            {
                _electCd = (int)(50 * 0.25f); // 0.25s 触发一次燃烧
                HurtComponent component = new HurtComponent()
                {
                    victimId = buff.id,
                    Element = 1,
                    ElementMeasure = 0,
                    Harm = 5,
                    HitType = 0,
                    Transform = buff.transform
                };
                HurtSystem.HurtSystem.Instance.EnQueue(component);
            }
            _electCd--;
            _val = Mathf.Max(0, _val - (int)(0.4 * 10000 / 50f)); // 2s 后反应结束
            IsFinished = _val == 0;
        }

        public void OnRemove()
        {
            
        }
    }
}