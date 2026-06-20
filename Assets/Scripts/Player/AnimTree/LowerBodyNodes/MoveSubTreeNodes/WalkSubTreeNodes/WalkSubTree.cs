using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 角色走路子树
    /// </summary>
    public class WalkSubTree : AnimTreeMixNode<IAnimContext>
    {
        public WalkSubTree(IAnimContext ctx) : base(ctx) { }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            bool res = !query.HasState(PlayerMoveFlags.Sprint);
            return parentActive && res;
        }
    }
}
