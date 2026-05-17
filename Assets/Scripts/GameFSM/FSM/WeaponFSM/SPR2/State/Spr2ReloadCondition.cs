using Firearms;

namespace GameFSM.FSM.WeaponFSM.SPR2.State
{
    /// <summary>
    /// Spr2 换弹判定
    /// </summary>
    public class Spr2ReloadCondition : AbstractCondition<AbstractGun>
    {
        public Spr2ReloadCondition(AbstractGun manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Reload";
            return MyManager.IsReload;
        }
    }
}