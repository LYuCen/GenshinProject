using Common.FSM.PlayerFSM;
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
        public CrouchMoveSubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new CrouchWalkSubTree(MyManager));
            AddNode(new CrouchSprintSubTree(MyManager));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyManager.inputMove != Vector2.zero;
    }
}