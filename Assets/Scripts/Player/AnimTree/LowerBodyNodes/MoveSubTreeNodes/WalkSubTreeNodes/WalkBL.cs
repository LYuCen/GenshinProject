using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    public class WalkBL : AnimTreeLeafNode<IAnimContext>
    {
        private AnimationClipPlayable _playable;

        public WalkBL(IAnimContext ctx) : base(ctx) { }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            bool res = query.HasState(PlayerMoveFlags.BackLeft);
            return parentActive && res;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var playable = AnimationClipPlayable.Create(graph, DefaultClip);
            playable.SetSpeed(this.AnimSpeed);
            return playable;
        }
    }
}
