using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes
{
    public class SlidingAimTreeNode : AnimTreeLeafNode<IAnimContext>
    {
        public SlidingAimTreeNode(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            return parentActive && query.HasState(PlayerMoveFlags.Sliding);
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var playable = AnimationClipPlayable.Create(graph, DefaultClip);
            playable.SetSpeed(this.AnimSpeed);
            return playable;
        }
    }
}
