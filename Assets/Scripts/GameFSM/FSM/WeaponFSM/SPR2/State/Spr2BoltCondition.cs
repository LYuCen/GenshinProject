using Firearms;

namespace GameFSM.FSM.WeaponFSM.SPR2.State
{
    /// <summary>
    /// Spr2 拉栓判定
    /// </summary>
    public class Spr2BoltCondition : AbstractCondition<AbstractGun>
    {
        public Spr2BoltCondition(AbstractGun manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Bolt";
            return MyManager.IsBolt;
        }
    }
}