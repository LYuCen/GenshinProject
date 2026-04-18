using Firearms.ThrowWeapon;
using GameFSM.FSM.PlayerFSM;
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
            if (MyMessage.firearmController.Weapon is Grenade g)
            {
                return parentActive && g.CurrentState != GrenadeState.Idle;
            }

            return false;
        }
    }
}