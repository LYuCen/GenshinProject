using GameFSM.FSM.PlayerFSM;
using UnityEngine;

namespace Player.AnimTree
{
    public class PlayerAnimController
    {
        [Range(0f, 1f)] public float LeftHandIk;
        [Range(0f, 1f)] public float RightHandIk;
        public bool OpenIk = true;
        private PlayerAnimTreeBuild _animBuild;

        public PlayerAnimController(Animator animator, PlayerManager manager, AvatarMask upperBodyMask, Transform leftHand,
            Transform rightHand)
        {
            _animBuild = new PlayerAnimTreeBuild(animator, manager, upperBodyMask, leftHand, rightHand);
        }
        
        public void Update()
        {
            _animBuild.Update(LeftHandIk, RightHandIk, OpenIk);
        }

        public void Destroy()
        {
            _animBuild.DestroyGraph();
        }
    }
}