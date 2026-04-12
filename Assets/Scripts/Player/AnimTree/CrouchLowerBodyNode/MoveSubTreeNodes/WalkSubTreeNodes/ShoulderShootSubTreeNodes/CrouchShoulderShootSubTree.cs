using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class CrouchShoulderShootSubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchShoulderShootSubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new CrouchShoulderShootWalkF(MyMessage));
            AddNode(new CrouchShoulderShootWalkFL(MyMessage));
            AddNode(new CrouchShoulderShootWalkFR(MyMessage));
            AddNode(new CrouchShoulderShootWalkL(MyMessage));
            AddNode(new CrouchShoulderShootWalkR(MyMessage));
            AddNode(new CrouchShoulderShootWalkBL(MyMessage));
            AddNode(new CrouchShoulderShootWalkBR(MyMessage));
            AddNode(new CrouchShoulderShootWalkB(MyMessage));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsShoot;
    }
}