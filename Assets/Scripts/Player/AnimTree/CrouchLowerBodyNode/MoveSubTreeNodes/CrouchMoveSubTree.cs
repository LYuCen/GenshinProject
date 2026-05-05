
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes
{
    /// <summary>
    /// 蹲移动子树
    /// </summary>
    public class CrouchMoveSubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchMoveSubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.InputMove != Vector2.zero;
    }
}