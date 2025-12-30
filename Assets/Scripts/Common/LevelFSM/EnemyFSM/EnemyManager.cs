using Enemy.Anim;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("当前状态")] public string stateStr;
        [Header("是否在地面")] public bool isGround;
        [Header("移动速度")] public float moveSpeed;
        [Header("左手ik")] public Transform leftHandIk;
        [Header("右手ik")] public Transform rightHandIk;
        
        [Header("角色动画组件")] public Animator animator;
        [Header("角色上下半身动画遮罩")] public AvatarMask avatarMask;
        
        
        public EnemyAnim EnemyAnimController; // 动画控制器

        public void Init()
        {
            EnemyAnimController = new EnemyAnim(animator, this, avatarMask, leftHandIk, rightHandIk);
        }

        private void Update()
        {
            EnemyAnimController.Update();
        }

        private void OnDisable()
        {
            EnemyAnimController.Destroy();
        }
    }
}