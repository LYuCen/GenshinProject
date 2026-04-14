using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes;

namespace Player.AnimTree.CrouchLowerBodyNode
{
    /// <summary>
    /// 蹲下半身动画树
    /// </summary>
    public class CrouchLowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchLowerBodySubTree(PlayerManager myManager, PlayerAnimSlot slot) : base(myManager)
        {
            CrouchIdleSlotTreeNode node = new CrouchIdleSlotTreeNode(MyMessage);
            slot.CIdleSlot = node;
            AddNode(node);
            AddNode(new CrouchMoveSubTree(MyMessage, slot));
            WeightSpeed = 3f;
        }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsCrouch;
    }
}