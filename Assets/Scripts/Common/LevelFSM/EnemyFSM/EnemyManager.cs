using Common.LevelFSM.EnemyFSM.LevelFSMState;
using Enemy.Anim;
using Firearms;
using Firearms.Gun;
using Firearms.Magazine;
using Firearms.WeaponInterface;
using mSystem.BuffSystem.ElementBuff;
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

        [Header("持枪手")] public Transform head;
        [Header("枪械")] public Transform gun;
        private IWeapon _weapon;

        public void Init()
        {
            

            AbstractMagazine magazine = new BaseMagazine(30, 30, 20, ElementEnum.Fire);
            
            _weapon = new XM7();
            _weapon.Init();
            gun = _weapon.GetWeapon();
            gun.SetParent(head);
            _weapon.InitTransform();
            if (_weapon is IReloadable reloadable)
            {
                reloadable.ReloadEnd(magazine);
            }

            leftHandIk = gun.Find("LeftHandIk");
            rightHandIk = leftHandIk;
            EnemyAnimController = new EnemyAnim(animator, this, avatarMask, leftHandIk, rightHandIk);
            EnemyAnimController.LeftHandIk = 1;
            EnemyAnimController.RightHandIk = 0;
        }

        private void Update()
        {
            EnemyAnimController.Update();
            moveSpeed = enemyAIController.GetCurrentSpeed();
            _weapon.UpdateWeapon();
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
                _weapon.UpdateForward(f);
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

        public void Shoot()
        {
            if (_weapon is AbstractGun abstractGun)
            {
                abstractGun.Shoot();
            }
        }

        private void OnDisable()
        {
            EnemyAnimController.Destroy();
        }
    }
}