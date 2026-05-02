using mSystem.HurtSystem;

namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    /// <summary>
    /// 感电持续反应效果逻辑
    /// </summary>
    public class ElectrifyEffect : IStatusEffect
    {
        private int _electCd;
        private int _electStep;
        public bool IsFinished { get; private set; }

        public void OnUpdate(ElementBuffController buff)
        {
            if (buff.Elements[2].Enable && buff.Elements[4].Enable && _electCd == 0)
            {
                _electCd = (int)(50 * 0.5f); // 0.5s 触发一次感电
                buff.ConsumeElement(2, 0.4f);
                buff.ConsumeElement(4, 0.4f);
                HurtComponent component = new HurtComponent()
                {
                    victimId = buff.id,
                    Element = 4,
                    ElementMeasure = 0,
                    Harm = 20,
                    HitType = 0,
                    Transform = buff.transform
                };
                HurtSystem.HurtSystem.Instance.EnQueue(component);
            }

            IsFinished = !buff.Elements[2].Enable || !buff.Elements[4].Enable;
            _electCd--;
        }

        public void OnRemove() { /* 清理特效 */ }
    }
}