using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public sealed class HighSubTree : AnimTreeMixNode<PlayerManager>
    {
        public HighSubTree(PlayerManager myManager) : base(myManager)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive && !MyMessage.firearmController.isDownThrow;
        }
    }
}