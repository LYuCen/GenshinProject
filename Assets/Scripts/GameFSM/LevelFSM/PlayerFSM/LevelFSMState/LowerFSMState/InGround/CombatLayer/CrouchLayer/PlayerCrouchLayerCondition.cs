using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer
{
    /// <summary>
    /// 玩家蹲姿状态判定 (Crouch)
    /// </summary>
    public class PlayerCrouchLayerCondition : AbstractCondition<PlayerManager>
    {
        public PlayerCrouchLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Crouch";
            PlayerMoveQuery query = MyManager.MoveModule.GetQuery();
            return query.HasState(PlayerMoveFlags.Crouch);
        }
    }
}