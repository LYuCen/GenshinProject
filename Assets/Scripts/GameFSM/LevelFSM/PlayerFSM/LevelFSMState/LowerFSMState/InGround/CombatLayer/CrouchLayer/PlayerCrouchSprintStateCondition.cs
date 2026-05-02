using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer
{
    /// <summary>
    /// 玩家蹲姿冲刺判定 (Sprint)
    /// </summary>
    public class PlayerCrouchSprintStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerCrouchSprintStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Sprint";
            return MyManager.IsSprint && (MyManager.InputMove == Vector2.up ||
                                          MyManager.InputMove == new Vector2(-1, 1) ||
                                          MyManager.InputMove == new Vector2(1, 1));
        }
    }
}