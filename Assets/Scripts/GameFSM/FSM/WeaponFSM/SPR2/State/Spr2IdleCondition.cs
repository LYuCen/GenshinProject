using Firearms;

namespace GameFSM.FSM.WeaponFSM.SPR2.State
{
    /// <summary>
    /// Spr2 待机判定
    /// </summary>
    public class Spr2IdleCondition : AbstractCondition<AbstractGun>
    {
        public Spr2IdleCondition(AbstractGun manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Idle";
            return !MyManager.IsReload && !MyManager.IsShoot && !MyManager.IsBolt;
        }
    }
}