using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes
{
    public class CrouchWalkSlotTreeNode : AnimTreeSlotNode<PlayerManager>
    {
        public CrouchWalkSlotTreeNode(PlayerManager myMessage) : base(myMessage)
        {
            
        }
    }
}