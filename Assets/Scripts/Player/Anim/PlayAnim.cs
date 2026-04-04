using GameFSM.FSM.PlayerFSM;
using UnityEngine;

namespace Player.Anim
{
    public class PlayAnim
    {
        [Range(0f, 1f)] public float LeftHandIk;
        [Range(0f, 1f)] public float RightHandIk;
        public Transform LeftHandEffector;
        public Transform RightHandEffector;
        public bool OpenIk = true;
        //private LowerBodyWeightController<PlayerManager> _lowerController = new ();
        //private UpperBodyWeightController<PlayerManager> _upperController = new();
        private PlayerAnimBuild _animBuild = new PlayerAnimBuild();
        private Animator _animator;
        private PlayerManager _manager;
        private AvatarMask _avatarMask;
        

        public PlayAnim(Animator animator, PlayerManager manager, AvatarMask avatarMask, Transform leftHand, Transform rightHand)
        {
            LeftHandEffector = leftHand;
            RightHandEffector = rightHand;
            _animBuild.Init(animator, manager, avatarMask, LeftHandEffector, RightHandEffector);
            //_lowerController.SetEnable(true);
            //_upperController.SetEnable(true);
        }
        
        public void ChangeLowerWeight(PlayerAnimState state)
        {
            //_lowerController.ChangeState(state);
        }

        public void ChangeUpperWeight(PlayerAnimState state)
        {
            //_upperController.ChangeState(state);
        }

        public void SetLowerWeightEnable(bool enable)
        {
            //_lowerController.SetEnable(enable);
        }

        public void SetUpperWeightEnable(bool enable)
        {
            //_upperController.SetEnable(enable);
        }
        
        public void Update()
        {
            //_lowerController.Update();
            //_upperController.Update();
            _animBuild.Update(LeftHandIk, RightHandIk, OpenIk);
        }

        public void Destroy()
        {
            _animBuild.DestroyGraph();
        }
    }
}