using GameFSM.FSM;

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
            return !MyManager.IsCrouch;
        }
    }
}