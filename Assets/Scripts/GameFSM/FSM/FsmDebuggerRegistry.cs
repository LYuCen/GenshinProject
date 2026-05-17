using System.Collections.Generic;

namespace GameFSM.FSM
{
    public static class FsmDebuggerRegistry
    {
        // 存储所有活跃的状态机调试接口
        // Key 可以是武器的名字或唯一 ID
        private static Dictionary<string, IFsmDebugger> _activeFsms = new();

        public static void Register(string name, IFsmDebugger fsm)
        {
            if (!_activeFsms.ContainsKey(name))
                _activeFsms.Add(name, fsm);
        }

        public static void Unregister(string name)
        {
            if (_activeFsms.ContainsKey(name))
                _activeFsms.Remove(name);
        }

        public static Dictionary<string, IFsmDebugger> GetAll() => _activeFsms;
    }
}