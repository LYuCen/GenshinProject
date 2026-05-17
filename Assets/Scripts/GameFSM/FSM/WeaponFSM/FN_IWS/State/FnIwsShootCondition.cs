using Firearms;

namespace GameFSM.FSM.WeaponFSM.FN_IWS.State
{
    /// <summary>
    /// 射击判定 Shoot
    /// </summary>
    public class FnIwsShootCondition : AbstractCondition<AbstractGun>
    {
        public FnIwsShootCondition(AbstractGun manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Shoot";
            return MyManager.IsShoot && !MyManager.IsReload;
        }
    }
}