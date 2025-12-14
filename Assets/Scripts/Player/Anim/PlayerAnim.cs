using Player.Ik;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim
{
    public class PlayerAnim : MonoBehaviour
    {
        public PlayableGraph Graph;
        public string animStr;
        public Animator animator;
        public AvatarMask avatarMask;
        public AnimationClip idleAnim;
        public AnimationClip walkAnim;
        public AnimationClip runAnim;
        public AnimationClip jumpAnim;
        public AnimationClip shootIdle;
        [Header("换弹动画")] public AnimationClip reloadClip;
        [Header("投掷动画")] public AnimationClip throwClip;
        [Header("瞄准八向移动")]
        public AnimationClip shootWalkForward;
        public AnimationClip shootWalkRightForward;
        public AnimationClip shootWalkRight;
        public AnimationClip shootWalkRightBack;
        public AnimationClip shootWalkBack;
        public AnimationClip shootWalkLeftBack;
        public AnimationClip shootWalkLeft;
        public AnimationClip shootWalkLeftForward;
        
        private AnimationMixerPlayable _mixer;
        private AnimationLayerMixerPlayable _layer;
        [Range(0f, 1f)] public float mixerWeight;
        [Range(0f, 1f)] public float layerWeight;
        [Range(0f, 1f)] public float leftHandIk;
        [Range(0f, 1f)] public float rightHandIk;
        public Transform leftHandEffector;
        public Transform rightHandEffector;
        public float speed;
        public bool openIk; // 是否开启 Ik
        public int index;       // 当前状态
        public int nextIndex;   // 下一个状态

        private AnimationScriptPlayable _jobPlayable;

        private void Start()
        {
            index = 0;
            nextIndex = 0;
            openIk = true;
            Graph = PlayableGraph.Create();
            _mixer = AnimationMixerPlayable.Create(Graph);
            AnimationClipPlayable idle = AnimationClipPlayable.Create(Graph, idleAnim);
            AnimationClipPlayable walk = AnimationClipPlayable.Create(Graph, walkAnim);
            AnimationClipPlayable run = AnimationClipPlayable.Create(Graph, runAnim);
            AnimationClipPlayable jump = AnimationClipPlayable.Create(Graph, jumpAnim);
            AnimationClipPlayable sIdle = AnimationClipPlayable.Create(Graph, shootIdle);
            
            // 八方向移动
            var sWalkForward = AnimationClipPlayable.Create(Graph, shootWalkForward);
            var sWalkRightForward = AnimationClipPlayable.Create(Graph, shootWalkRightForward);
            var sWalkRight = AnimationClipPlayable.Create(Graph, shootWalkRight);
            var sWalkRightBack = AnimationClipPlayable.Create(Graph, shootWalkRightBack);
            var sWalkBack = AnimationClipPlayable.Create(Graph, shootWalkBack);
            var sWalkLeftBack = AnimationClipPlayable.Create(Graph, shootWalkLeftBack);
            var sWalkLeft = AnimationClipPlayable.Create(Graph, shootWalkLeft);
            var sWalkLeftForward = AnimationClipPlayable.Create(Graph, shootWalkLeftForward);
            
            
            _mixer.AddInput(idle, 0, 1f);
            _mixer.AddInput(walk, 0);
            _mixer.AddInput(sIdle, 0);
            _mixer.AddInput(sWalkForward, 0);
            _mixer.AddInput(sWalkRightForward, 0);
            _mixer.AddInput(sWalkRight, 0);
            _mixer.AddInput(sWalkRightBack, 0);
            _mixer.AddInput(sWalkBack, 0);
            _mixer.AddInput(sWalkLeftBack, 0);
            _mixer.AddInput(sWalkLeft, 0);
            _mixer.AddInput(sWalkLeftForward, 0);
            _mixer.AddInput(run, 0);
            _mixer.AddInput(jump, 0);
            
            _mixer.SetPropagateSetTime(true); // 传播时间改为 true 使混合时间和动画时间保持一致
            
            // 换弹
            _layer = AnimationLayerMixerPlayable.Create(Graph, 2);
            var reload = AnimationClipPlayable.Create(Graph, reloadClip);
            var throwAnim = AnimationClipPlayable.Create(Graph, throwClip);
            Graph.Connect(_mixer, 0, _layer, 0);
            Graph.Connect(reload, 0, _layer, 1);
            _layer.SetLayerMaskFromAvatarMask(1, avatarMask);
            _layer.SetInputWeight(0, 1);


            BodyIkJob job = new BodyIkJob();
            job.LeftHand = animator.BindSceneTransform(leftHandEffector);
            job.LeftWeight = leftHandIk;
            job.RightHand = animator.BindSceneTransform(rightHandEffector);
            job.RightWeight = rightHandIk;
            _jobPlayable = AnimationScriptPlayable.Create(Graph, job);
            
            idle.SetApplyFootIK(false); // Ik 后处理
            idle.SetApplyPlayableIK(false);
            _jobPlayable.AddInput(_layer, 0, 1f);
            
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(Graph, "Player", animator);
            output.SetSourcePlayable(_jobPlayable);
            //Graph.Play();
        }

        private void Update()
        {
            _mixer.SetInputWeight(index, 1 - mixerWeight);
            _mixer.SetInputWeight(nextIndex, mixerWeight);
            _mixer.SetSpeed(speed);

            _layer.SetInputWeight(1, layerWeight);
            
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

        public void RemoveWeight()
        {
            _mixer.SetInputWeight(index, 0);
        }

        public void AwakeAnim(int animIndex)
        {
            index = nextIndex;
            _mixer.SetInputWeight(index, 1);
            nextIndex = animIndex;
            mixerWeight = 0;
        }

        public void ReloadAnim()
        {
            // 拿到 reload Playable
            var reloadPlayable = (AnimationClipPlayable)_layer.GetInput(1);

            // 重置动画时间到 0
            reloadPlayable.SetTime(0);

            // 清空内部缓存（强制刷新时间）
            reloadPlayable.Pause();   // 先暂停
            reloadPlayable.Play();    // 再播放

            // 确保速度正常
            reloadPlayable.SetSpeed(1);
        }

        private void OnDisable()
        {
            Graph.Destroy();
        }
    }
}