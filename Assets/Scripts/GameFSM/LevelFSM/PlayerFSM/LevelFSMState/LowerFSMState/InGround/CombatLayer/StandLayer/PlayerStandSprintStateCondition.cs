using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer
{
    /// <summary>
    /// 玩家站立冲刺判定 (Sprint)
    /// </summary>
    public class PlayerStandSprintStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerStandSprintStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
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