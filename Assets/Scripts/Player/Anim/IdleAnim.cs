using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim
{
    public struct LeftHandIkJob : IAnimationJob
    {
        public TransformSceneHandle LeftHandHandle;
        public float Weight;
        
        public void ProcessAnimation(AnimationStream stream)
        {
            if (stream.isValid && LeftHandHandle.IsValid(stream))
            {
                var human = stream.AsHuman();
                
                human.SetGoalPosition(AvatarIKGoal.LeftHand,LeftHandHandle.GetPosition(stream));
                human.SetGoalRotation(AvatarIKGoal.LeftHand,LeftHandHandle.GetRotation(stream));
                human.SetGoalWeightPosition(AvatarIKGoal.LeftHand, Weight);
                human.SetGoalWeightRotation(AvatarIKGoal.LeftHand, Weight);
                human.SolveIK(); // 计算 Ik
            }
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            
        }
    }
    
    public class IdleAnim : MonoBehaviour
    {
        public Animator animator;
        public AnimationClip clip;
        public Transform ikTransform;
        [Range(0f, 1f)] public float weight;

        private PlayableGraph _graph;
        private AnimationScriptPlayable _jobPlayable;

        private void Start()
        {
            _graph = PlayableGraph.Create();

            LeftHandIkJob job = new LeftHandIkJob();
            job.LeftHandHandle = animator.BindSceneTransform(ikTransform);
            job.Weight = weight;
            _jobPlayable = AnimationScriptPlayable.Create(_graph, job);

            AnimationClipPlayable anim = AnimationClipPlayable.Create(_graph, clip);
            anim.SetApplyFootIK(false); // 表示 Ik 我们后面再去处理
            anim.SetApplyPlayableIK(false);
            _jobPlayable.AddInput(anim, 0, 1f);

            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Anim", animator);
            output.SetSourcePlayable(_jobPlayable);
            _graph.Play();
        }

        private void OnDisable()
        {
            _graph.Destroy();
        }
    }
}