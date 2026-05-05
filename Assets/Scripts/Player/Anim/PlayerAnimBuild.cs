using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM;
using Player.Ik;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim
{
    public class PlayerAnimBuild
    {
        private PlayableGraph _graph;
        private Animator _animator;
        private PlayerManager _manager;
        private AnimState<PlayerManager> _root;
        private Playable _rootPlayable;
        private AvatarMask _avatarMask;
        private AnimationScriptPlayable _jobPlayable;
        

        public void Init(Animator animator, PlayerManager manager, AvatarMask avatarMask, Transform leftHand, Transform rightHand)
        {
            _animator = animator;
            _manager = manager;
            _avatarMask = avatarMask;
            
            _graph = PlayableGraph.Create("PlayerPlayAble");
            _root = new AnimRoot(_manager, _avatarMask);
            _rootPlayable = _root.BuildNode(_graph);
            
            BodyIkJob job = new BodyIkJob();
            job.LeftHand = animator.BindSceneTransform(leftHand);
            job.LeftWeight = 0;
            job.RightHand = animator.BindSceneTransform(rightHand);
            job.RightWeight = 0;
            _jobPlayable = AnimationScriptPlayable.Create(_graph, job);
            
            _jobPlayable.AddInput(_rootPlayable, 0, 1f);
            
            var output = AnimationPlayableOutput.Create(_graph, "AnimOutput", _animator);
            output.SetSourcePlayable(_jobPlayable);
            _graph.Play();
        }

        public void Update(float leftHandIk, float rightHandIk, bool openIk)
        {
            _root.NextState();
            
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
            _graph.Destroy();
        }
    }

    public class AnimRoot : PlayerAnimState
    {
        private AnimationLayerMixerPlayable _rootLayer;
        private AvatarMask _avatarMask;
        public AnimRoot(PlayerManager myManager, AvatarMask avatarMask) : base(myManager)
        {
            //AddState(new LowerBodyAnimState(MyManager));
            //AddState(new UpperBodyAnimState(MyManager));
            _avatarMask = avatarMask;
        }

        protected override bool CheckState()
        {
            return true;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override void NextState()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].NextState();
            }
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            int inputCount = Children.Count;
            MixIndex = mixIndex;
            _rootLayer = AnimationLayerMixerPlayable.Create(graph, inputCount);
            for (int i = 0; i < inputCount; i++)
            {
                Playable playable = Children[i].BuildNode(graph, i);
                graph.Connect(playable, 0, _rootLayer, i);
            }

            _rootLayer.SetInputWeight(0, 1f);
            _rootLayer.SetInputWeight(1, 1f);
            _rootLayer.SetLayerMaskFromAvatarMask(1, _avatarMask);
            return _rootLayer;
        }

        public override void OnExitState()
        {
            
        }
    }
}