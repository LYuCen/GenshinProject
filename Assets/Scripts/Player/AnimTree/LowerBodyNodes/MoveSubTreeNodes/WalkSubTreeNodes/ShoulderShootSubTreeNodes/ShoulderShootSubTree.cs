using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    /// <summary>
    /// 肩射瞄准子树
    /// </summary>
    public class ShoulderShootSubTree : AnimTreeMixNode<PlayerManager>
    {
        public ShoulderShootSubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new ShoulderShootWalkF(MyMessage));
            AddNode(new ShoulderShootWalkFL(MyMessage));
            AddNode(new ShoulderShootWalkFR(MyMessage));
            AddNode(new ShoulderShootWalkL(MyMessage));
            AddNode(new ShoulderShootWalkR(MyMessage));
            AddNode(new ShoulderShootWalkBL(MyMessage));
            AddNode(new ShoulderShootWalkBR(MyMessage));
            AddNode(new ShoulderShootWalkB(MyMessage));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsShoot;
    }
}