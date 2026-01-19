using Common.LevelFSM.EnemyFSM.LevelFSMState;
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
        [Header("敌人感知半径")] public float perceptionRange;
        [Header("敌人攻击半径")] public float attackRange;
        [Header("脱离攻击状态半径")] public float outAttackRange;
        [Header("敌人进入攻击范围游走半径")] public float randomRange;
        
        [Header("是否处于攻击状态")] public bool isAttack;
        [Header("下一次决策时间")] public float choiceTimer;
        [Header("决策时间间隔")] public float choiceCd; // 移动选择 cd
        [Header("是否在追击状态")] public bool isFollow;
        
        public EnemyAnim EnemyAnimController; // 动画控制器
        public EnemyAI enemyAIController; // 决策控制器
        public LayerMask playerLayerMask;
        private Collider[] _hits = new Collider[4];

        public void Init()
        {
            EnemyAnimController = new EnemyAnim(animator, this, avatarMask, leftHandIk, rightHandIk);
        }

        private void Update()
        {
            EnemyAnimController.Update();
            moveSpeed = enemyAIController.GetCurrentSpeed();
        }

        public Transform GetPlayerTrans()
        {
            int count = Physics.OverlapSphereNonAlloc( 
                transform.position, 
                perceptionRange, 
                _hits, 
                playerLayerMask
                );
            if (count > 0) // 如果检测到玩家就朝向玩家
            {
                Vector3 f = _hits[0].transform.position - transform.position;
                f.y = 0;
                Quaternion targetRot = Quaternion.LookRotation(f);
                    transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    Time.deltaTime * 10f
                );
            }
            return count == 0 ? null : _hits[0].transform;
        }

        private void OnDisable()
        {
            EnemyAnimController.Destroy();
        }
    }
}