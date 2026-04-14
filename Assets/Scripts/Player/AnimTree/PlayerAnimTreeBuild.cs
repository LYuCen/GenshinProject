using GameFSM.FSM.PlayerFSM;
using Player.Ik;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree
{
    public class PlayerAnimTreeBuild
    {
        private PlayableGraph _graph;
        private PlayerAnimTreeRoot _rootNode; // 使用新架构的根节点
        private AnimationScriptPlayable _jobPlayable;

        public PlayerAnimTreeBuild(Animator animator, PlayerManager manager, AvatarMask upperBodyMask, Transform leftHand,
            Transform rightHand, PlayerAnimSlot slot)
        {
            _graph = PlayableGraph.Create("PlayerPlayablePro");
            

            // 1. 构建动画树逻辑
            _rootNode = new PlayerAnimTreeRoot(manager, upperBodyMask, slot);
            Playable animTreeOutput = _rootNode.BuildNode(_graph);

            // 2. 构建 IK Job
            BodyIkJob job = new BodyIkJob
            {
                LeftHand = animator.BindSceneTransform(leftHand),
                LeftWeight = 0,
                RightHand = animator.BindSceneTransform(rightHand),
                RightWeight = 0,
            };
            
            _jobPlayable = AnimationScriptPlayable.Create(_graph, job);
            _jobPlayable.SetInputCount(1);
            
            // 3. 将动画树的输出作为 IK Job 的输入
            _graph.Connect(animTreeOutput, 0, _jobPlayable, 0);
            _jobPlayable.SetInputWeight(0, 1f);

            // 4. 设置输出
            var output = AnimationPlayableOutput.Create(_graph, "AnimOutput", animator);
            output.SetSourcePlayable(_jobPlayable);
            
            _graph.Play();
        }
        
        
        /// <summary>
        /// 更新动画 IK 权重
        /// </summary>
        /// <param name="leftHandIk">左手 IK</param>
        /// <param name="rightHandIk">右手 IK</param>
        /// <param name="openIk">是否打开 IK</param>
        public void Update(float leftHandIk, float rightHandIk, bool openIk)
        {
            // 更新动画树逻辑
            _rootNode.Evaluate();
            _rootNode.UpdateNode();

            // 更新 IK 数据
            BodyIkJob job = _jobPlayable.GetJobData<BodyIkJob>();
            if (openIk)
            {
                job.LeftWeight = leftHandIk;
                job.RightWeight = rightHandIk;
            }
            else
            {
                job.LeftWeight = 0;
                job.RightWeight = 0;
            }
            
            _jobPlayable.SetJobData(job);
        }

        public void DestroyGraph()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}