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
            AddNode(new ShoulderShootWalkF(MyManager));
            AddNode(new ShoulderShootWalkFL(MyManager));
            AddNode(new ShoulderShootWalkFR(MyManager));
            AddNode(new ShoulderShootWalkL(MyManager));
            AddNode(new ShoulderShootWalkR(MyManager));
            AddNode(new ShoulderShootWalkBL(MyManager));
            AddNode(new ShoulderShootWalkBR(MyManager));
            AddNode(new ShoulderShootWalkB(MyManager));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyManager.IsShoot;
    }
}