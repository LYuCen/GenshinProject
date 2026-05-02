using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 蹲下走路移动子树
    /// </summary>
    public class CrouchWalkSubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchWalkSubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.IsSprint;
    }
}