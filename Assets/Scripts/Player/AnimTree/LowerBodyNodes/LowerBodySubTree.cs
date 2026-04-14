using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes;

namespace Player.AnimTree.LowerBodyNodes
{
    // 下半身/全身 Mixer：负责基础移动逻辑切换
    public class LowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public LowerBodySubTree(PlayerManager myManager, PlayerAnimSlot slot) : base(myManager)
        {
            IdleSlotTreeNode idleSlot = new IdleSlotTreeNode(MyMessage);
            slot.IdleSlot = idleSlot;
            AddNode(idleSlot);
            AddNode(new MoveSubTree(MyMessage, slot));
            AddNode(new RifleSlidingTreeLeafNode(MyMessage));
            WeightSpeed = 3f;
        }

        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.IsCrouch;
    }
}