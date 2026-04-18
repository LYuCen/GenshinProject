using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class CrouchShoulderShootSubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchShoulderShootSubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsShoot;
    }
}