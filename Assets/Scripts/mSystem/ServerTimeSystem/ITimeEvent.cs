namespace mSystem.ServerTimeSystem
{
    /// <summary>
    /// 时间事件
    /// </summary>
    public interface ITimeEvent
    {
        double TriggerTime { get; }
        void Execute();
    }
}