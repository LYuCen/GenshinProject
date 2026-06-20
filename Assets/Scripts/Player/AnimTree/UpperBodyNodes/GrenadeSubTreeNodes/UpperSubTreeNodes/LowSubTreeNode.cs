using mInterface;
using Player.AnimPro;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class LowSubTreeNode : AnimTreeMixNode<IAnimContext>
    {

        public LowSubTreeNode(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive;
            //return parentActive && MyMessage.firearmController.isDownThrow;
        }
    }
}
