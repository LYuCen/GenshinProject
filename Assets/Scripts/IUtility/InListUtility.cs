using System.Collections.Generic;

namespace IUtility
{
    public static class InListUtility
    {
        public static bool IsInList<T>(List<T> list,int index)
        {
            return index >= 0 && index < list.Count;
        }
    }
}