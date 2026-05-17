using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer
{
    /// <summary>
    /// 玩家蹲姿行走判定 (Walk)
    /// </summary>
    public class PlayerCrouchWalkStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerCrouchWalkStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            PlayerMoveQuery query = MyManager.MoveModule.GetQuery();
            return !query.HasState(PlayerMoveFlags.Sprint);
        }
    }
}