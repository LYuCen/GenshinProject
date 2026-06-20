using Firearms.ThrowWeapon;
using mInterface;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class HighThrowStartTreeLeafNode : AnimTreeLeafNode<IAnimContext>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;

        public HighThrowStartTreeLeafNode(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return false;
        }

        public override void OnEnterState()
        {
        }

        public override float UpdateNode()
        {
            float w = base.UpdateNode();
            if (!_playable.IsValid()) return w;

            float normTime = (float)(_playable.GetTime() / _clipDuration);

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
