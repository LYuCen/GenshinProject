using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 角色走路子树
    /// </summary>
    public class WalkSubTree : AnimTreeMixNode<PlayerManager>
    {
        public WalkSubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            bool res = !query.HasState(PlayerMoveFlags.Sprint);
            return parentActive && res;
        }
    }
}