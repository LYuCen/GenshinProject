using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player
{
    public struct FootIkJob : IAnimationJob
    {

        public TransformSceneHandle LeftFootHandle;
        public float Weight;
        
        public void ProcessAnimation(AnimationStream stream)
        {
            if (stream.isValid && LeftFootHandle.IsValid(stream)) // 判断是否合法
            {
                var human = stream.AsHuman();
                // 设置位置和旋转角权重
                human.SetGoalPosition(AvatarIKGoal.LeftFoot,LeftFootHandle.GetPosition(stream));
                human.SetGoalRotation(AvatarIKGoal.LeftFoot,LeftFootHandle.GetRotation(stream));
                human.SetGoalWeightPosition(AvatarIKGoal.LeftFoot,Weight);
                human.SetGoalWeightRotation(AvatarIKGoal.LeftFoot,Weight);
                human.SolveIK(); // 计算 Ik 
            }
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            
        }
    }
    
    public class IkSample : MonoBehaviour
    {
        public Animator animator;   // 动画控制器
        public AnimationClip clip;  // 动画
        public Transform leftFootEffector;  // Ik Transform
        [Range(0f, 1f)] public float weight;    // 权重

        private PlayableGraph _graph;
        private AnimationScriptPlayable _jobPlayable;

        private void Start()
        {
            _graph = PlayableGraph.Create();

            FootIkJob job = new FootIkJob();
            job.LeftFootHandle = animator.BindSceneTransform(leftFootEffector);
            job.Weight = weight;
            _jobPlayable =  AnimationScriptPlayable.Create(_graph, job);

            AnimationClipPlayable anim = AnimationClipPlayable.Create(_graph, clip);
            anim.SetApplyFootIK(false); // 表示 Ik 我们后面再去处理
            anim.SetApplyPlayableIK(false);
            _jobPlayable.AddInput(anim, 0, 1f);
            
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Anim", animator);
            output.SetSourcePlayable(_jobPlayable);
            _graph.Play();
        }

        private void Update()
        {
            FootIkJob job = _jobPlayable.GetJobData<FootIkJob>();
            job.Weight = weight;
            _jobPlayable.SetJobData(job);
        }

        private void OnDisable()
        {
            _graph.Destroy();
        }
    }
}