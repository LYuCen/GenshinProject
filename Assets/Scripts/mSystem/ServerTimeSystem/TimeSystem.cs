using System.Collections.Generic;
using BaseClass;
using UnityEngine;

namespace mSystem.ServerTimeSystem
{
    /// <summary>
    /// 事件调度器 — FixedUpdate 自动驱动
    /// </summary>
    public class TimeSystem : AbstractSingleton<TimeSystem>
    {
        private readonly List<ITimeEvent> _events = new();

        public void Add(ITimeEvent e)
        {
            _events.Add(e);
        }

        public void Tick(double now)
        {
            for (int i = _events.Count - 1; i >= 0; i--)
            {
                if (now >= _events[i].TriggerTime)
                {
                    _events[i].Execute();
                    _events.RemoveAt(i);
                }
            }
        }

        private void FixedUpdate()
        {
            Tick(Time.fixedTime);
        }
    }
}