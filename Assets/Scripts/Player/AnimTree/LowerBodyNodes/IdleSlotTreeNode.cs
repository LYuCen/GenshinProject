using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes
{
    public class IdleSlotTreeNode : AnimTreeSlotNode<PlayerManager>
    {
        public IdleSlotTreeNode(PlayerManager myMessage) : base(myMessage)
        {
            
        }
    }
}