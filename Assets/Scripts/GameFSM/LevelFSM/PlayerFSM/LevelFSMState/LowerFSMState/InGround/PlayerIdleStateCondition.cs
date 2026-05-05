using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround
{
    /// <summary>
    /// 玩家待机判定 (Idle)
    /// </summary>
    public class PlayerIdleStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerIdleStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Idle";
            return MyManager.InputMove.sqrMagnitude <= 0.001f;
        }
    }
}