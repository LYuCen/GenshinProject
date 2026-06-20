using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes
{
    public class RifleSlidingTreeLeafNode : AnimTreeLeafNode<IAnimContext>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;

        public RifleSlidingTreeLeafNode(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            return parentActive && query.HasState(PlayerMoveFlags.Sliding);
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            if (!DefaultClip)
            {
                Debug.LogError($"[ReloadNode] 找不到有效的动画剪辑！Tag: {Tag}");
                return Playable.Null;
            }

            _playable = AnimationClipPlayable.Create(graph, DefaultClip);
            _playable.SetSpeed(this.AnimSpeed);

            _clipDuration = DefaultClip.length;

            return _playable;
        }

        public override void OnEnterState()
        {
            _playable.SetTime(0);
        }

        public override float UpdateNode()
        {
            float currentWeight = base.UpdateNode();
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();

            if (currentWeight > 0.5f && query.HasState(PlayerMoveFlags.Sliding))
            {
                if (_playable.GetTime() >= _clipDuration)
                {

                }
            }
            return currentWeight;
        }
    }
}
