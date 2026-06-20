using mInterface;
using Player.AnimPro;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public sealed class HighSubTree : AnimTreeMixNode<IAnimContext>
    {
        public HighSubTree(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive;
        }
    }
}
