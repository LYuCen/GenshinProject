using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;
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
            PlayerMoveQuery query = MyManager.MoveModule.GetQuery();
            return query.HasState(PlayerMoveFlags.Sprint);
        }
    }
}