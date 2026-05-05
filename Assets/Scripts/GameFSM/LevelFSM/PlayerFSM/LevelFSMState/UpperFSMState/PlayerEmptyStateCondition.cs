using System.Collections.Generic;
using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 角色上半身空状态判定 (Empty)
    /// </summary>
    public class PlayerEmptyStateCondition : AbstractCondition<PlayerManager>
    {
        private List<AbstractCondition<PlayerManager>> _list;
        public PlayerEmptyStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
            _list = new List<AbstractCondition<PlayerManager>>();
            _list.Add(new PlayerAimStateCondition(MyManager, "Aim"));
            _list.Add(new PlayerReloadStateCondition(MyManager, "Reload"));
            _list.Add(new PlayerThrowStateCondition(MyManager, "Throw"));
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Empty";
            bool res = false;
            for (int i = 0; i < _list.Count; i++)
            {
                res = res || _list[i].CheckCondition(out _);
            }
            return !res;
        }
    }
}