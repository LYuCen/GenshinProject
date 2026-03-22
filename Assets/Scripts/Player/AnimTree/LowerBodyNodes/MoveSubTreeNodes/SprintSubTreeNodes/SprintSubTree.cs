using Common.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes
{
    /// <summary>
    /// 冲刺移动子树
    /// </summary>
    public class SprintSubTree : AnimTreeMixNode<PlayerManager>
    {
        public SprintSubTree(PlayerManager myManager) : base(myManager)
        {
            AddState(new RifleSprintFTreeLeafNode(MyManager));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyManager.isSprint;
    }
}