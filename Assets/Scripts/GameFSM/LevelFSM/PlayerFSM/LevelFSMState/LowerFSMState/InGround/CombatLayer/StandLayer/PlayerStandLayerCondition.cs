using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer
{
    /// <summary>
    /// 玩家站立层判定 (Stand)
    /// </summary>
    public class PlayerStandLayerCondition : AbstractCondition<PlayerManager>
    {
        public PlayerStandLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Stand";
            PlayerMoveQuery query = MyManager.MoveModule.GetQuery();
            return !query.HasState(PlayerMoveFlags.Crouch);
        }
    }
}