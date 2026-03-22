using Common.FSM.PlayerFSM;
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
            AddState(new RifleWalkTreeLeafNode(MyManager));
            AddState(new ShoulderShootSubTree(MyManager));
        }
        
        

        public override bool CheckState(bool parentActive) => parentActive && !MyManager.isSprint;
    }
}