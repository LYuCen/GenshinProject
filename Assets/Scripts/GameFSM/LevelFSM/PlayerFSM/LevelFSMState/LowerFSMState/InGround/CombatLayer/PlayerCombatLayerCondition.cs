using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;
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
            PlayerMoveQuery query = MyManager.MoveModule.GetQuery();
            return query.HasState(PlayerMoveFlags.Move);
        }
    }
}