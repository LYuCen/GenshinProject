using Firearms.ThrowWeapon;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes
{
    public class InThrowLowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public InThrowLowerBodySubTree(PlayerManager myManager) : base(myManager)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            

            return false;
        }
    }
}