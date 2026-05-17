
using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
using Player.AnimPro;

namespace Player.AnimTree.CrouchLowerBodyNode
{
    /// <summary>
    /// 蹲下半身动画树
    /// </summary>
    public class CrouchLowerBodySubTree : AnimTreeMixNode<PlayerManager>
    {
        public CrouchLowerBodySubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            bool res = query.HasState(PlayerMoveFlags.Crouch);
            return parentActive && res;
        }
    }
}