using Firearms.ThrowWeapon;

using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes
{
    public class NotThrowLowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public NotThrowLowerBodySubTree(PlayerManager myManager) : base(myManager)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            

            return false;
        }
    }
}