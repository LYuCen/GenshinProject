using Firearms.ThrowWeapon;
using mInterface;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes
{
    public class InThrowLowerBodySubTree : AnimTreeMixNode<IAnimContext>
    {
        public InThrowLowerBodySubTree(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {


            return false;
        }
    }
}
