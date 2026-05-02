using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 角色走路子树
    /// </summary>
    public class WalkSubTree : AnimTreeMixNode<PlayerManager>
    {
        public WalkSubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.IsSprint;
    }
}