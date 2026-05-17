using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家瞄准状态判定 (Aim)
    /// </summary>
    public class PlayerAimStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerAimStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }


        public override bool CheckCondition(out string nextState)
        {
            nextState = "Aim";
            WeaponQuery query = MyManager.CombatModule.GetQuery();
            return query.HasState(WeaponFlags.Shoot);
        }
    }
}