using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 角色走路子树
    /// </summary>
    public class WalkSubTree : AnimTreeMixNode<PlayerManager>
    {
        public WalkSubTree(PlayerManager myManager, PlayerAnimSlot slot) : base(myManager)
        {
            WalkSlotTreeNode node = new WalkSlotTreeNode(MyMessage);
            slot.WalkSlot = node;
            AddNode(node);
            AddNode(new ShoulderShootSubTree(MyMessage));
        }
        
        

        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.IsSprint;
    }
}