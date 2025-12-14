using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player
{
    public class RuntimeControllerSample : MonoBehaviour
    {
        public Animator animator;
        public AnimationClip clip1;
        public RuntimeAnimatorController controller;
        [Range(0f, 1f)] public float weight;   // 权重
        
        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;

        private void Start()
        {
            _graph = PlayableGraph.Create();
            
            _mixer = AnimationMixerPlayable.Create(_graph);
            AnimationClipPlayable anim1 = AnimationClipPlayable.Create(_graph, clip1);
            AnimatorControllerPlayable ctrl = AnimatorControllerPlayable.Create(_graph, controller);
            _mixer.AddInput(anim1, 0, 1f);
            _mixer.AddInput(ctrl, 0);
            
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Anim", animator);
            output.SetSourcePlayable(_mixer);
            _graph.Play();
        }

        private void Update()
        {
            // 修改两个动画权重
            _mixer.SetInputWeight(0,1 - weight);
            _mixer.SetInputWeight(1, weight);
        }

        private void OnDisable()
        {
            _graph.Destroy();
        }
    }
}