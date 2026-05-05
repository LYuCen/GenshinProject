
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class LowSubTreeNode : AnimTreeMixNode<PlayerManager>
    {
        
        public LowSubTreeNode(PlayerManager myManager) : base(myManager)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive && MyMessage.firearmController.isDownThrow;
        }
    }
}