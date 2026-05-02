using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes
{
    /// <summary>
    /// 角色移动子树
    /// </summary>
    public class MoveSubTree : AnimTreeMixNode<PlayerManager>
    {
        public MoveSubTree(PlayerManager myManager) : base(myManager)
        {
        }

        public override bool CheckState(bool parentActive)
        { 
            return parentActive && MyMessage.PlayerMove.sqrMagnitude > 0.001f;
        }
    }
}