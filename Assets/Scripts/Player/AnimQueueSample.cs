using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player
{
    public class AnimQueueSample : MonoBehaviour
    {
        public Animator animator;
        public AnimationClip[] clips;

        private PlayableGraph _graph;

        private void Start()
        {
            _graph = PlayableGraph.Create();

            var animQueuePlayable = ScriptPlayable<AnimQueue>.Create(_graph);
            animQueuePlayable.GetBehaviour().Init(clips, animQueuePlayable, _graph);
            
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Anim", animator);
            output.SetSourcePlayable(animQueuePlayable);
            _graph.Play();
        }

        private void OnDisable()
        {
            _graph.Destroy();
        }
    }
    
    public class AnimQueue : PlayableBehaviour
    {
        private AnimationMixerPlayable _mixer;
        
        private int _currentIndex = 0;
        private float _currentLenght;

        public void Init(AnimationClip[] clips, Playable playable, PlayableGraph graph)
        {
            _mixer = AnimationMixerPlayable.Create(graph);
            foreach (var clip in clips)
            {
                _mixer.AddInput(AnimationClipPlayable.Create(graph, clip), 0);
            }
            _mixer.SetInputWeight(0,1f);
            _currentLenght = clips[0].length;
            playable.AddInput(_mixer, 0, 1f);
            
        }

        public override void PrepareFrame(Playable playable, FrameData info) // 每帧前一秒调用
        {
            base.PrepareFrame(playable, info);

            if (_mixer.GetInput(_currentIndex).GetTime() >= _currentLenght) 
            {
                if (_currentIndex < _mixer.GetInputCount()) // 如果到时间换下一个动画播放
                {
                    _mixer.SetInputWeight(_currentIndex++,0);
                    _mixer.SetInputWeight(_currentIndex,1);
                    var current = _mixer.GetInput(_currentIndex);
                    current.SetTime(0);
                    current.SetTime(0);
                    _currentLenght = ((AnimationClipPlayable)current).GetAnimationClip().length;
                }
            }
        }
    }
}
