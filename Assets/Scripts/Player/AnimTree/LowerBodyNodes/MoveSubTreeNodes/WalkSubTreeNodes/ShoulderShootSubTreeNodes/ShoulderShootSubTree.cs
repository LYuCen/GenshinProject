using Common.FSM.PlayerFSM;
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
            AddState(new ShoulderShootWalkF(MyManager));
            AddState(new ShoulderShootWalkFL(MyManager));
            AddState(new ShoulderShootWalkFR(MyManager));
            AddState(new ShoulderShootWalkL(MyManager));
            AddState(new ShoulderShootWalkR(MyManager));
            AddState(new ShoulderShootWalkBL(MyManager));
            AddState(new ShoulderShootWalkBR(MyManager));
            AddState(new ShoulderShootWalkB(MyManager));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyManager.isShoot;
    }
}