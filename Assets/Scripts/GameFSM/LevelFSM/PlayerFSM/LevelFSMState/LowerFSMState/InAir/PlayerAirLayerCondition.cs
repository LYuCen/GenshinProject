using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir
{
    /// <summary>
    /// 玩家滞空层判定 (Air)
    /// </summary>
    public class PlayerAirLayerCondition : AbstractCondition<PlayerManager>
    {
        public PlayerAirLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Air";
            PlayerMoveQuery query = MyManager.MoveModule.GetQuery();
            return !query.HasState(PlayerMoveFlags.Ground);
        }
    }
}