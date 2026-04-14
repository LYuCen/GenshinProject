using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 蹲下走路移动子树
    /// </summary>
    public class CrouchWalkSubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchWalkSubTree(PlayerManager myManager, PlayerAnimSlot slot) : base(myManager)
        {
            CrouchWalkSlotTreeNode node = new CrouchWalkSlotTreeNode(MyMessage);
            slot.CWalkSlot = node;
            AddNode(node);
            AddNode(new CrouchShoulderShootSubTree(MyMessage));
        }
        
        

        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.IsSprint;
    }
}