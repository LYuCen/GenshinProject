using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.ThrowAnim.BaseMove
{
    public class ThrowBaseMoveAnim : PlayerAnimState
    {
        private int _prevIndex = -1;
        private int _nextIndex = -1;
        private AnimationMixerPlayable _mixer;
        public ThrowBaseMoveAnim(PlayerManager myManager) : base(myManager)
        {
            AddState(new ThrowBaseWalk(myManager));
            AddState(new ThrowBaseRun(myManager));
        }

        protected override bool CheckState()
        {
            return !MyManager.isShoot && MyManager.inputMove != Vector2.zero;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            int inputCount = Children.Count;
            MixIndex = mixIndex;
            _mixer = AnimationMixerPlayable.Create(graph, inputCount);
            for (int i = 0; i < inputCount; i++)
            {
                Children[i].Parent = this;
                Playable playable = Children[i].BuildNode(graph, i);
                graph.Connect(playable, 0, _mixer, i);
                _mixer.SetInputWeight(i, 0);
            }

            return _mixer;
        }

        public override void OnExitState()
        {
            
        }

        public override void SetWeight(int index, float weight = 0)
        {
            // 第一次设置，直接指定
            if (_nextIndex == -1)
            {
                _nextIndex = index;
                _mixer.SetInputWeight(index, weight);
                return;
            }

            // 更新索引
            if (index != _nextIndex)
            {
                if(_prevIndex != -1) _mixer.SetInputWeight(_prevIndex, 0);
                _prevIndex = _nextIndex;
                _nextIndex = index;
            }

            // 平滑过渡：新状态 weight, 旧状态 1 - weight
            if (_prevIndex != -1)
            {
                _mixer.SetInputWeight(_prevIndex, 1f - weight);
            }

            _mixer.SetInputWeight(_nextIndex, weight);
        }
    }
}