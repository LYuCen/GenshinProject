using System.Collections.Generic;

namespace GameFSM.FSM
{
    public interface IFsmDebugger
    {
        string GetCurrentStateName();
        IEnumerable<string> GetAllStateNames();
        IEnumerable<string> GetConditions(string stateName);
    }
}