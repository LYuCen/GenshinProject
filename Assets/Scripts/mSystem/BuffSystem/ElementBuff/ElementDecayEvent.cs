using mSystem.ServerTimeSystem;

namespace mSystem.BuffSystem.ElementBuff
{
    /// <summary>
    /// 元素衰减到零时触发，设置 Enable = false
    /// </summary>
    public class ElementDecayEvent : ITimeEvent
    {
        public double TriggerTime { get; private set; }

        private readonly ElementBuffController _controller;
        private readonly int _elementIndex;

        public ElementDecayEvent(ElementBuffController controller, int elementIndex, double now)
        {
            _controller = controller;
            _elementIndex = elementIndex;

            var e = controller.Elements[elementIndex];
            float rate = ElementData.GetDecayRate(e.Level);
            float currentVal = e.GetVal((float)now);
            float timeToZero = rate > 0 ? currentVal / rate : float.MaxValue;
            TriggerTime = now + timeToZero;
        }

        public void Execute()
        {
            var data = _controller.Elements[_elementIndex];
            if (!data.Enable) return;

            if (data.GetVal((float)TriggerTime) <= 0)
            {
                data.Enable = false;
                _controller.Elements[_elementIndex] = data;
            }
        }
    }
}
