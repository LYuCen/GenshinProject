using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes
{
    /// <summary>
    /// 角色移动子树
    /// </summary>
    public class MoveSubTree : AnimTreeMixNode<IAnimContext>
    {
        public MoveSubTree(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();

            // 在地面且八方向移动
            bool res = query.HasState(PlayerMoveFlags.Ground) && query.HasState(PlayerMoveFlags.Move);
            return parentActive && res;
        }
    }
}
