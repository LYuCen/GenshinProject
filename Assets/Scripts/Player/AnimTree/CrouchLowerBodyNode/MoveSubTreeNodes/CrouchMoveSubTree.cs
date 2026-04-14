using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.SprintSubTreeNodes;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes;
using UnityEngine;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes
{
    /// <summary>
    /// 蹲移动子树
    /// </summary>
    public class CrouchMoveSubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchMoveSubTree(PlayerManager myManager, PlayerAnimSlot slot) : base(myManager)
        {
            AddNode(new CrouchWalkSubTree(MyMessage, slot));
            CrouchSprintSlotTreeNode node = new CrouchSprintSlotTreeNode(MyMessage);
            slot.CSprintSlot = node;
            AddNode(node);
        }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.InputMove != Vector2.zero;
    }
}