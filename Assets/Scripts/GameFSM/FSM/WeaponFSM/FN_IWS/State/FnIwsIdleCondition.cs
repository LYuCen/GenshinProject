using Firearms;

namespace GameFSM.FSM.WeaponFSM.FN_IWS.State
{
    /// <summary>
    /// 武器待机判定 Idle
    /// </summary>
    public class FnIwsIdleCondition : AbstractCondition<AbstractGun>
    {
        public FnIwsIdleCondition(AbstractGun manager, string nextStateName) : base(manager, nextStateName)
        {
            
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Idle";
            return !MyManager.IsReload && !MyManager.IsShoot;
        }
    }
}