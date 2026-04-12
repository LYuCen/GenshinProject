using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.SprintSubTreeNodes
{
    /// <summary>
    /// 蹲下冲刺移动子树
    /// </summary>
    public class CrouchSprintSubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchSprintSubTree(PlayerManager myManager) : base(myManager)
        {
            AddNode(new CrouchRifleSprintTreeLeafNode(MyMessage));
            AddNode(new CrouchThrowSprintTreeLeafNode(MyMessage));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsSprint;
    }
}