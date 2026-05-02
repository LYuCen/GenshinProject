using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround
{
    /// <summary>
    /// 玩家地面层判定 (Ground)
    /// </summary>
    public class PlayerGroundLayerCondition : AbstractCondition<PlayerManager>
    {
        public PlayerGroundLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Ground";
            return MyManager.IsGround;
        }
    }
}