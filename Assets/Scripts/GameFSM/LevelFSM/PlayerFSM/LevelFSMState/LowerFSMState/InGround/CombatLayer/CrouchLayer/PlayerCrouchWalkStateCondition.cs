using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer
{
    /// <summary>
    /// 玩家蹲姿行走判定 (Walk)
    /// </summary>
    public class PlayerCrouchWalkStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerCrouchWalkStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            return !MyManager.IsSprint;
        }
    }
}