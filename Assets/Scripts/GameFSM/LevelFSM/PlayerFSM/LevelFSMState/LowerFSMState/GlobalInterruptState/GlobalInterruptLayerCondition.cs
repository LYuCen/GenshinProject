using GameFSM.FSM;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.GlobalInterruptState
{
    /// <summary>
    /// 全局打断层判定 GlobalInterrupt
    /// </summary>
    public class GlobalInterruptLayerCondition : AbstractCondition<PlayerManager>
    {
        public GlobalInterruptLayerCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
            
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "GlobalInterrupt";
            return MyManager.HpModule.IsDead();
        }
    }
}