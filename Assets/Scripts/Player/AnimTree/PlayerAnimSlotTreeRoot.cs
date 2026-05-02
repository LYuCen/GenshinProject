using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree
{
    public class PlayerAnimSlotTreeRoot : AnimTreeSlotNode<PlayerManager>
    {
        public PlayerAnimSlotTreeRoot(PlayerManager myManager) : base(myManager)
        {
            // 根节点逻辑通常始终为 true
        }

        public override bool CheckState(bool parentActive) => true;

        // 注意：因为根节点是 Slot，你需要提供一个方法来安装“默认的主动画树”
        public void SetupMainTree(AnimGraphData graphData, PlayerAnimSlot slot)
        {
            if (!graphData) return;
        
            // 调用解析器，并把 slot 传进去进行 Tag 绑定
            var tree = AnimGraphRuntimeBuilder.Build(graphData, MyMessage, slot);
            this.InstallSubtree(tree);
        }
    }
}