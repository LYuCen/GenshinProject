using mInterface;
using Player.AnimPro;

namespace Player.AnimTree
{
    public class PlayerAnimSlotTreeRoot : AnimTreeSlotNode<IAnimContext>
    {
        public PlayerAnimSlotTreeRoot(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive) => true;
        
        public void SetupMainTree(AnimGraphData graphData, PlayerAnimSlot slot)
        {
            if (!graphData) return;
            
            var tree = AnimGraphRuntimeBuilder.Build(graphData, MyMessage, slot);
            this.InstallSubtree(tree);
        }
    }
}