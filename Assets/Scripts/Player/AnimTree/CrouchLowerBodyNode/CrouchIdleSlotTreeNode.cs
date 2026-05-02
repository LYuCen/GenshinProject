using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode
{
    public class CrouchIdleSlotTreeNode : AnimTreeSlotNode<PlayerManager>
    {
        public CrouchIdleSlotTreeNode(PlayerManager myMessage) : base(myMessage)
        {
            
        }
    }
}