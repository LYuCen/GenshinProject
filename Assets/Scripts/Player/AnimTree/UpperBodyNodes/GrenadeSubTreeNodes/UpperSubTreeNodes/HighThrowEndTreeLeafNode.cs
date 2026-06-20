using Firearms.ThrowWeapon;
using mInterface;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class HighThrowEndTreeLeafNode : AnimTreeLeafNode<IAnimContext>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;
        public HighThrowEndTreeLeafNode(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return false;
        }

        public override void OnEnterState()
        {
            if (!_playable.IsValid()) return;
        }

        public override float UpdateNode()
        {
            float w = base.UpdateNode();
            if (!_playable.IsValid()) return w;

            return w;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            _playable = AnimationClipPlayable.Create(graph, DefaultClip);
            _playable.SetSpeed(this.AnimSpeed);

            if (DefaultClip) _clipDuration = DefaultClip.length;
            return _playable;
        }
    }
}
