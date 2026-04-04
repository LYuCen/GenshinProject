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
        public WalkSubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new RifleWalkTreeLeafNode(MyManager));
            AddNode(new ShoulderShootSubTree(MyManager));
            AddNode(new ThrowWalkTreeLeafNode(MyManager));
        }
        
        

        public override bool CheckState(bool parentActive) => parentActive && !MyManager.IsSprint;
    }
}