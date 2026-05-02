using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes
{
    public class MixTrueTreeNode : AnimTreeMixNode<PlayerManager>
    {
        public MixTrueTreeNode(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return true;
        }
    }
}