using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround
{
    /// <summary>
    /// 玩家地面层判定 (Ground)
    /// </summary>
    public class PlayerGroundLayerCondition : AbstractCondition<PlayerManager>
    {
        public PlayerGroundLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Ground";
            PlayerMoveQuery query = MyManager.MoveModule.GetQuery();
            return query.HasState(PlayerMoveFlags.Ground) && !MyManager.HpModule.IsDead();
        }
    }
}