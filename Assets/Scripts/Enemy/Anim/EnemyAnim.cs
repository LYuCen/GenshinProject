using Common.LevelFSM.EnemyFSM;
using Player.Anim;
using Player.Anim.LowerBodyAnim;
using Player.Anim.UpperBodyAnim;
using UnityEngine;

namespace Enemy.Anim
{
    public class EnemyAnim
    {
        [Range(0f, 1f)] public float LeftHandIk;
        [Range(0f, 1f)] public float RightHandIk;
        public Transform LeftHandEffector;
        public Transform RightHandEffector;
        public bool OpenIk = true;
        private LowerBodyWeightController<EnemyManager> _lowerController = new ();
        private UpperBodyWeightController<EnemyManager> _upperController = new();
        private EnemyAnimBuild _animBuild = new EnemyAnimBuild();
        private Animator _animator;
        private EnemyManager _manager;
        private AvatarMask _avatarMask;
        

        public EnemyAnim(Animator animator, EnemyManager manager, AvatarMask avatarMask, Transform leftHand, Transform rightHand)
        {
            LeftHandEffector = leftHand;
            RightHandEffector = rightHand;
            _animBuild.Init(animator, manager, avatarMask, LeftHandEffector, RightHandEffector);
            _lowerController.SetEnable(true);
            _upperController.SetEnable(true);
        }
        
        public void ChangeLowerWeight(EnemyAnimState state)
        {
            _lowerController.ChangeState(state);
        }

        public void ChangeUpperWeight(EnemyAnimState state)
        {
            _upperController.ChangeState(state);
        }

        public void SetLowerWeightEnable(bool enable)
        {
            _lowerController.SetEnable(enable);
        }

        public void SetUpperWeightEnable(bool enable)
        {
            _upperController.SetEnable(enable);
        }
        
        public void Update()
        {
            _lowerController.Update();
            _upperController.Update();
            _animBuild.Update(LeftHandIk, RightHandIk, OpenIk);
        }

        public void Destroy()
        {
            _animBuild.DestroyGraph();
        }
    }
}