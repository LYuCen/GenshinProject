using Common.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes;

namespace Player.AnimTree.CrouchLowerBodyNode
{
    /// <summary>
    /// 蹲下半身动画树
    /// </summary>
    public class CrouchLowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchLowerBodySubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new CrouchIdleTreeLeafNode(MyManager));
            AddNode(new CrouchThrowIdleTreeLeafNode(MyManager));
            AddNode(new CrouchMoveSubTree(MyManager));
            WeightSpeed = 3f;
        }

        public override bool CheckState(bool parentActive) => parentActive && MyManager.isCrouch;
    }
}