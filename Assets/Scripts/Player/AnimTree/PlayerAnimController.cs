using GameFSM.FSM.PlayerFSM;
using Player.AnimTree.CrouchLowerBodyNode;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.SprintSubTreeNodes;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes;
using Player.AnimTree.LowerBodyNodes;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes;
using Player.AnimTree.UpperBodyNodes;
using UnityEngine;

namespace Player.AnimTree
{
    public class PlayerAnimController
    {
        [Range(0f, 1f)] public float LeftHandIk;
        [Range(0f, 1f)] public float RightHandIk;
        public bool OpenIk = true;
        private PlayerAnimTreeBuild _animBuild;
        public PlayerAnimSlot Slot;

        public PlayerAnimController(Animator animator, PlayerManager manager, AvatarMask upperBodyMask, Transform leftHand,
            Transform rightHand)
        {
            Slot = new PlayerAnimSlot();
            _animBuild = new PlayerAnimTreeBuild(animator, manager, upperBodyMask, leftHand, rightHand, Slot);
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

    public class PlayerAnimSlot
    {
        public UpperSlotTreeNode UpperSlot;
        public IdleSlotTreeNode IdleSlot;
        public WalkSlotTreeNode WalkSlot;
        public SprintSlotTreeNode SprintSlot;

        public CrouchIdleSlotTreeNode CIdleSlot;
        public CrouchWalkSlotTreeNode CWalkSlot;
        public CrouchSprintSlotTreeNode CSprintSlot;
    }
}