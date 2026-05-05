using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer
{
    /// <summary>
    /// 玩家站立行走判定 (Walk)
    /// </summary>
    public class PlayerStandWalkStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerStandWalkStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            return !MyManager.IsSprint;
        }
    }
}