using Firearms;

namespace GameFSM.FSM.WeaponFSM.SPR2.State
{
    /// <summary>
    /// Spr2 射击判定
    /// </summary>
    public class Spr2ShootCondition : AbstractCondition<AbstractGun>
    {
        public Spr2ShootCondition(AbstractGun manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Shoot";
            return MyManager.IsShoot && !MyManager.IsReload;
        }
    }
}