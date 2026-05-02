using GameFSM.FSM;

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
            return MyManager.IsCrouch;
        }
    }
}