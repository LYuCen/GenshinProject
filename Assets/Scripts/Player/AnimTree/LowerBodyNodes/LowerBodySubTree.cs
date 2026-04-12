using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes;

namespace Player.AnimTree.LowerBodyNodes
{
    // 下半身/全身 Mixer：负责基础移动逻辑切换
    public class LowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public LowerBodySubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new IdleTreeLeafNode(MyMessage));
            AddNode(new MoveSubTree(MyMessage));
            AddNode(new RifleSlidingTreeLeafNode(MyMessage));
            AddNode(new ThrowIdleTreeLeafNode(MyMessage));
            WeightSpeed = 3f;
        }

        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.IsCrouch;
    }
}