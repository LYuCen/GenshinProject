
using mInterface;
using Player.AnimPro;

namespace Player.AnimTree
{
    public class PlayerAnimTreeRoot : AnimTreeLayerMixNode<IAnimContext>
    {
        public PlayerAnimTreeRoot(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return true;
        }
    }
}