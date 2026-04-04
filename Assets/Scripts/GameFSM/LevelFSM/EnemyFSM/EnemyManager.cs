using AI;
using AI.EqsController;
using BaseClass;
using Common.LevelFSM.EnemyFSM.LevelFSMState;
using Enemy;
using Enemy.Anim;
using Firearms;
using Firearms.Gun;
using Firearms.Magazine;
using Firearms.WeaponInterface;
using mController;
using mSystem.BuffSystem.ElementBuff;
using UnityEngine;
using UnityEngine.Rendering;

namespace Common.LevelFSM.EnemyFSM
{
    public class EnemyManager : MonoBehaviour, IManager
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
        [Header("敌人状态")] public BodyController bodyController;
        [Header("是否为全局打断状态")] public bool isGlobalInterrupt;
        [Header("敌人是否处于战斗状态")] public bool isCombat;
        [Header("敌人是否在寻找掩体")] public bool isTryMoveCover;
        [Header("敌人是否在射击状态")] public bool isStrafeShoot;
        [Header("是否被攻击")] public bool isHurt;
        [Header("AI寻路")] public EnemyPatrol EnemyPatrol;
        [Header("AI视觉")] public AISightPerception AISightPerception;
        [Header("网格Eqs")] public NavMeshReachable GridEqsController;
        [Header("圆周Eqs")] public CircleEqsControllerTest CircleEqsControllerTest;

        public Material pureMaterial;
        public EnemyAnim EnemyAnimController; // 动画控制器
        public EnemyAI enemyAIController; // 决策控制器
        public LayerMask playerLayerMask;
        private Collider[] _hits = new Collider[4];

        [Header("持枪手")] public Transform head;
        [Header("枪械")] public Transform gun;
        private IWeapon _weapon;
        [SerializeField] private GameObject diePrefab;

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
            
            DieParticleObjectPool.Instance.Init(diePrefab);
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
            return count == 0 ? null : _hits[0].transform;
        }

        public void Shoot()
        {
            if (AISightPerception.sightTarget && _weapon is AbstractGun abstractGun)
            {
                _weapon.UpdateForward((AISightPerception.sightPoint - transform.position).normalized);
                abstractGun.Shoot();
            }
        }

        private void OnDestroy()
        {
            EnemyAnimController.Destroy();
        }

        public void UpdateHp(float val)
        {
            bodyController.LoseHp((int)val);
            if (bodyController.hp > 0) isHurt = true;
            isGlobalInterrupt = true;
        }

        public void CreateSnapshot()
        {
            GameObject root = new GameObject("Snapshot");
            ShowDieParticle sdp = root.AddComponent<ShowDieParticle>();
            sdp.Action = _ =>
            {
                root.gameObject.SetActive(false);
                GameObject obj = DieParticleObjectPool.Instance.GetObj();
                obj.transform.position = root.transform.position + Vector3.up;
                obj.SetActive(true);
            };

            root.transform.SetPositionAndRotation(transform.position, transform.rotation);

            var smrs = GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var smr in smrs)
            {
                Mesh mesh = new Mesh();
                mesh.indexFormat = IndexFormat.UInt32;

                smr.BakeMesh(mesh);

                GameObject go = new GameObject(smr.name);
                go.transform.SetParent(root.transform);

                go.transform.SetPositionAndRotation(smr.transform.position, smr.transform.rotation);
                go.transform.localScale = Vector3.one;

                var mf = go.AddComponent<MeshFilter>();
                var mr = go.AddComponent<MeshRenderer>();

                mf.sharedMesh = mesh;

                // 关键：处理多个SubMesh
                int subMeshCount = mesh.subMeshCount;

                Material[] mats = new Material[subMeshCount];

                for (int i = 0; i < subMeshCount; i++)
                {
                    mats[i] = pureMaterial;
                }

                mr.materials = mats;
            }
        }
    }
}