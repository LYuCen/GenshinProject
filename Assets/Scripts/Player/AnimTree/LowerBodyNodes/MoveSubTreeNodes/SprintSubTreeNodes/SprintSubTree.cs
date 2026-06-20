using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;
using UnityEngine;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes
{
    public class SprintSubTree : AnimTreeMixNode<IAnimContext>
    {
        public SprintSubTree(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            // 处于奔跑状态并且方向为 前，前左，前右
            bool res = query.HasState(PlayerMoveFlags.Sprint) &&
                       (query.HasState(PlayerMoveFlags.Forward) ||
                        query.HasState(PlayerMoveFlags.ForwardLeft) ||
                        query.HasState(PlayerMoveFlags.ForwardRight));
            return parentActive && res;
        }
    }
}
