namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    public class SharpenEffect : IStatusEffect
    {
        public bool IsFinished { get; private set; }
        private int _val = 50 * 10; // 10s
        public void OnUpdate(ElementBuffController buff)
        {
            _val--;
            IsFinished = _val <= 0;
        }

        public void OnRemove()
        {
            
        }
    }
}