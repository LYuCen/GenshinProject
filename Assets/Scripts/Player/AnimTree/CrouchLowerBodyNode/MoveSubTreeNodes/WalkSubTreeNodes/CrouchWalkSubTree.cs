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
        public CrouchWalkSubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new CrouchRifleWalkTreeLeafNode(MyManager));
            AddNode(new CrouchShoulderShootSubTree(MyManager));
            AddNode(new CrouchThrowWalkTreeLeafNode(MyManager));
        }
        
        

        public override bool CheckState(bool parentActive) => parentActive && !MyManager.IsSprint;
    }
}