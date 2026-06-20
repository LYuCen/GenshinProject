using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.GlobalInterruptState
{
    /// <summary>
    /// 角色死亡判定
    /// </summary>
    public class PlayerDeadCondition : AbstractCondition<PlayerManager>
    {
        public PlayerDeadCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Dead";
            return MyManager.HpModule.IsDead();
        }
    }
}