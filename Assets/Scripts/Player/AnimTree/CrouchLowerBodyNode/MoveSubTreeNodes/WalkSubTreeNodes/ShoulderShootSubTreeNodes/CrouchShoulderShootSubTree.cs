using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class CrouchShoulderShootSubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchShoulderShootSubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new CrouchShoulderShootWalkF(MyManager));
            AddNode(new CrouchShoulderShootWalkFL(MyManager));
            AddNode(new CrouchShoulderShootWalkFR(MyManager));
            AddNode(new CrouchShoulderShootWalkL(MyManager));
            AddNode(new CrouchShoulderShootWalkR(MyManager));
            AddNode(new CrouchShoulderShootWalkBL(MyManager));
            AddNode(new CrouchShoulderShootWalkBR(MyManager));
            AddNode(new CrouchShoulderShootWalkB(MyManager));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyManager.IsShoot;
    }
}