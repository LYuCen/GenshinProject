using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;
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
            PlayerMoveQuery query = MyManager.MoveModule.GetQuery();
            return query.HasState(PlayerMoveFlags.Sprint);
        }
    }
}