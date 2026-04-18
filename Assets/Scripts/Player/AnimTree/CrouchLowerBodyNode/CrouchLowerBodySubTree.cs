using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode
{
    /// <summary>
    /// 蹲下半身动画树
    /// </summary>
    public class CrouchLowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchLowerBodySubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsCrouch;
    }
}