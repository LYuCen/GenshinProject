using Common.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes;
using UnityEngine;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes
{
    /// <summary>
    /// 角色移动子树
    /// </summary>
    public class MoveSubTree : AnimTreeMixNode<PlayerManager>
    {
        public MoveSubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new WalkSubTree(MyManager));
            AddNode(new SprintSubTree(MyManager));
        }

        public override bool CheckState(bool parentActive)
        { 
            return parentActive && MyManager.inputMove != Vector2.zero && !MyManager.isSliding;
        }
    }
}