using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes
{
    public class RifleSlidingTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;

        public RifleSlidingTreeLeafNode(PlayerManager manager) : base(manager)
        {
            WeightSpeed = 8.0f;
        }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsSliding;

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Sliding.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            _clipDuration = cfg.animClip.length;
            return _playable;
        }

        public override void OnEnterState()
        {
            _playable.SetTime(0); // 每次进入时重置时间
        }

        public override float UpdateNode()
        {
            float currentWeight = base.UpdateNode();

            // 逻辑：如果当前权重较大（已切入），且动画播放完毕，则重置状态
            if (currentWeight > 0.5f && MyMessage.IsSliding)
            {
                if (_playable.GetTime() >= _clipDuration)
                {
                    MyMessage.slidingCd = 0.5f;
                    MyMessage.IsSliding = false;
                }
            }
            return currentWeight;
        }
    }
}