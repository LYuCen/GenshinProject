using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player
{
    /// <summary>
    /// 动画上下半分离加混合
    /// </summary>
    public class LayerMixerSample : MonoBehaviour
    {
        public Animator animator;
        public AnimationClip idleAnim;
        public AnimationClip walkAnim;
        public AnimationClip reloadAnim;
        public AvatarMask avatarMask;
        [Range(0f, 1f)] public float layerWeight;
        [Range(0f, 1f)] public float mixerWeight;
        private PlayableGraph _graph;
        private AnimationLayerMixerPlayable _layer;
        private AnimationMixerPlayable _mixer;
        private void Start()
        {
            _graph = PlayableGraph.Create("layerGraph");

            animator.applyRootMotion = false;
            
            _mixer = AnimationMixerPlayable.Create(_graph, 2);
            var idle = AnimationClipPlayable.Create(_graph, idleAnim);
            var walk = AnimationClipPlayable.Create(_graph, walkAnim);
            _graph.Connect(idle, 0, _mixer, 0);
            _graph.Connect(walk, 0, _mixer, 1);
            
            _layer = AnimationLayerMixerPlayable.Create(_graph, 2);
            var reload = AnimationClipPlayable.Create(_graph, reloadAnim);
            _graph.Connect(_mixer, 0, _layer, 0);
            _graph.Connect(reload, 0, _layer, 1);
            _layer.SetLayerMaskFromAvatarMask(1, avatarMask);
            _layer.SetInputWeight(0, 1);
            
            reload.SetSpeed(2f);


            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "output", animator);
            output.SetSourcePlayable(_layer);
            _graph.Play();
        }

        private void Update()
        {
            _mixer.SetInputWeight(0, 1 - mixerWeight);
            _mixer.SetInputWeight(1, mixerWeight);

            _layer.SetInputWeight(1, layerWeight);
        }

        private void OnDisable()
        {
            _graph.Destroy();
        }
    }
}