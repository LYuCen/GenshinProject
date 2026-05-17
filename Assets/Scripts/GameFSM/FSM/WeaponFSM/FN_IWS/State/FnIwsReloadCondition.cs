using Firearms;

namespace GameFSM.FSM.WeaponFSM.FN_IWS.State
{
    /// <summary>
    /// 换弹判定 Reload
    /// </summary>
    public class FnIwsReloadCondition : AbstractCondition<AbstractGun>
    {
        public FnIwsReloadCondition(AbstractGun manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Reload";
            return MyManager.IsReload;
        }
    }
}