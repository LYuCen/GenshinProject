using Firearms.ThrowWeapon;

using mInterface;
using Player.AnimPro;


namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes
{
    public sealed class GrenadeThrowSubTree : AnimTreeMixNode<IAnimContext>
    {

        public GrenadeThrowSubTree(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            /*
            if (MyMessage.firearmController.Weapon is Grenade g)
            {
                return parentActive && g.CurrentState != GrenadeState.Idle;
            }
            */
            return false;
        }
    }
}
