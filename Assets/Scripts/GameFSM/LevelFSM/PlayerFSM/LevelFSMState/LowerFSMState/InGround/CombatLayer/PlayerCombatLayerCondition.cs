using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer
{
    /// <summary>
    /// 玩家战斗判定 (Combat)
    /// </summary>
    public class PlayerCombatLayerCondition : AbstractCondition<PlayerManager>
    {
        public PlayerCombatLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Combat";
            return MyManager.InputMove.sqrMagnitude > 0.001f;
        }
    }
}