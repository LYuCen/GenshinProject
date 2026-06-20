using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;

namespace Player.AnimTree.LowerBodyNodes
{
    // 下半身/全身 Mixer：负责基础移动逻辑切换
    public class LowerBodySubTree : AnimTreeMixNode<IAnimContext>
    {
        public LowerBodySubTree(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            bool condition = !query.HasState(PlayerMoveFlags.Crouch);
            return parentActive && condition;
        }
    }
}
