namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    /// <summary>
    /// 元素反应效果
    /// </summary>
    public interface IStatusEffect 
    {
        bool IsFinished { get; }
        void OnUpdate(ElementBuffController buff);
        void OnRemove();
    }
}