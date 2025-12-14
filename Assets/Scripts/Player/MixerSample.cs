using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player
{
    /// <summary>
    /// 动画混合
    /// </summary>
    public class MixerSample : MonoBehaviour
    {
        public Animator animator;
        public AnimationClip clip1;
        public AnimationClip clip2;
        public bool openAnimTime;
        public float time; // 动画时间
        public float speed; // 动画速度
        [Range(0f, 1f)] public float weight;   // 权重
        
        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;
        private bool _isPaused;

        private void Start()
        {
            _graph = PlayableGraph.Create();
            
            //_mixer = AnimationMixerPlayable.Create(_graph,2);
            _mixer = AnimationMixerPlayable.Create(_graph);
            AnimationClipPlayable anim1 = AnimationClipPlayable.Create(_graph, clip1);
            AnimationClipPlayable anim2 = AnimationClipPlayable.Create(_graph, clip2);
            //_graph.Connect(anim1, 0, _mixer, 0);
            //_graph.Connect(anim2, 0, _mixer, 1);
            //_mixer.SetInputWeight(0,1f);
            _mixer.AddInput(anim1, 0, 1f);
            _mixer.AddInput(anim2, 0);
            _mixer.SetPropagateSetTime(true); // 传播时间改为 true 使混合时间和动画时间保持一致
            
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Anim", animator);
            output.SetSourcePlayable(_mixer);
            _graph.Play();
        }

        private void Update()
        {
            // 修改两个动画权重
            _mixer.SetInputWeight(0,1 - weight);
            _mixer.SetInputWeight(1, weight);
            
            if(openAnimTime) _mixer.SetTime(time);
            _mixer.SetSpeed(speed);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isPaused = !_isPaused;
                if (_isPaused) // 实现动画暂停
                {
                    _mixer.SetSpeed(1f);
                }
                else
                {
                    _mixer.SetSpeed(0f);
                }
            }
        }

        private void OnDisable()
        {
            _graph.Destroy();
        }
    }
}