using Common.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes;

namespace Player.AnimTree.LowerBodyNodes
{
    // 下半身/全身 Mixer：负责基础移动逻辑切换
    public class LowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public LowerBodySubTree(PlayerManager myManager) : base(myManager)
        {
            AddState(new IdleTreeLeafNode(MyManager));
            AddState(new MoveSubTree(MyManager));
        }

        public override bool CheckState(bool parentActive) => parentActive;
    }
}