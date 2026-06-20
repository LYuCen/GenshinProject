using Firearms.ThrowWeapon;

using mInterface;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes
{
    public class NotThrowLowerBodySubTree : AnimTreeMixNode<IAnimContext>
    {
        public NotThrowLowerBodySubTree(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {


            return false;
        }
    }
}
