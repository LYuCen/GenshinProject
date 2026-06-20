using Firearms.ThrowWeapon;

using mInterface;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class HighThrowLoopTreeLeafNode : AnimTreeLeafNode<IAnimContext>
    {
        private AnimationClipPlayable _playable;
        public HighThrowLoopTreeLeafNode(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            _playable = AnimationClipPlayable.Create(graph, DefaultClip);
            _playable.SetSpeed(this.AnimSpeed);
            return _playable;
        }
    }
}
