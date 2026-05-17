using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家投掷状态判定 (Throw)
    /// </summary>
    public class PlayerThrowStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerThrowStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Throw";
            WeaponQuery query = MyManager.CombatModule.GetQuery();
            return query.HasState(WeaponFlags.Throw);
        }
    }
}