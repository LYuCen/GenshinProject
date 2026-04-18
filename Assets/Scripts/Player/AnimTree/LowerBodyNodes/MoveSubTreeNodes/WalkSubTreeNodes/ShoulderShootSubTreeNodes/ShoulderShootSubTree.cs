using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    /// <summary>
    /// 肩射瞄准子树
    /// </summary>
    public class ShoulderShootSubTree : AnimTreeMixNode<PlayerManager>
    {
        public ShoulderShootSubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsShoot;
    }
}