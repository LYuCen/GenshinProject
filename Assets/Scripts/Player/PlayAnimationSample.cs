using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player
{
    public class PlayAnimationSample : MonoBehaviour
    {
        public Animator animator;
        public AnimationClip clip;

        private PlayableGraph _graph;

        private void Start()
        {
            _graph = PlayableGraph.Create();
            
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime); // 更新模式为跟随游戏时间一起更新
            
            AnimationClipPlayable anim = AnimationClipPlayable.Create(_graph, clip);
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Anim", animator);
            output.SetSourcePlayable(anim);
            
            _graph.Play();
        }

        private void OnDisable()
        {
            _graph.Destroy();
        }
    }
}
