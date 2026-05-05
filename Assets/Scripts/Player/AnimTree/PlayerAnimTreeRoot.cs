
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree
{
    public class PlayerAnimTreeRoot : AnimTreeLayerMixNode<PlayerManager>
    {
        public PlayerAnimTreeRoot(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return true;
        }
    }
}