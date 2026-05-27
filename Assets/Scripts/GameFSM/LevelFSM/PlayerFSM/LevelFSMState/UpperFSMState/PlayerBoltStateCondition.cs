using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家拉栓状态判定 (Bolt)
    /// </summary>
    public class PlayerBoltStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerBoltStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Bolt";
            WeaponQuery query = MyManager.CombatModule.GetQuery();
            return query.HasState(WeaponFlags.Bolt);
        }
    }
}