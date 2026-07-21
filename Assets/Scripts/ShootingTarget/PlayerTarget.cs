using System;
using BaseClass;
using Connect.Data;
using GameFSM.FSM.PlayerTargetFSM;
using mInterface;
using mSystem.HurtSystem;
using mSystem.StatusSystem;
using Player;
using Player.AnimPro;
using Server;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShootingTarget
{
    public class PlayerTarget : MonoBehaviour, IHittable, IManager
    {
        [SerializeField] private EntityStatusUI statusUI;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform weaponPivot;
        [SerializeField] private PlayerStateData data;
        [SerializeField] private CharacterEffectManager effectManager;
        [SerializeField] private AnimGraphData graphData;
        [SerializeField] private CharacterController characterController;
        [Header("死亡材质")] public Material deathSnapshotMaterial;
        private ServerAnimContext _context;

        private ServerMoveProvider _moveProvider;
        private ServerCombatProvider _combatProvider;
        private TargetAnimController _animController;

        public PlayerStateData Data => data;
        public float moveSpeed;

        private PlayerTargetFsm _fsm;

        public Action<PlayerTarget> Dead;

        private float _originalMoveSpeed;

        private void Start()
        {
            _originalMoveSpeed = moveSpeed;

            statusUI.InitStatus(data);

            _moveProvider = new ServerMoveProvider(data);
            _combatProvider = new ServerCombatProvider(data);

            _context = new ServerAnimContext(_moveProvider, _combatProvider, weaponPivot, animator);

            _animController = new TargetAnimController(graphData, _context);

            _fsm = new PlayerTargetFsm(new PlayerTargetFsmBuilder(this));
        }
        
        public void Reset()
        {
            data.maxHp = 200;
            data.maxShield = 100;
            data.curHp = 200;
            data.curShield = 100;
            data.isGround = true;
            data.isCrouch = false;
            data.inputMove = Vector2.zero;
            data.playerMove = Vector2.zero;
            statusUI.InitStatus(data);
            moveSpeed = _originalMoveSpeed;
            gameObject.SetActive(true);
            _fsm?.SetDefaultState("Idle");
            moveSpeed = _originalMoveSpeed;
            effectManager.ResetMaterialAlpha(0);
        }

        public void OnHit(HitInfo info)
        {
            var hurtComponent = new HurtComponent
            {
                attackerId = -1,
                victimId = -1,
                Harm = info.Damage,
                Element = info.Element,
                ElementMeasure = info.ElementMeasure,
                HitType = (int)info.Part,
                Transform = transform
            };

            HurtSystem.Instance.EnQueue(hurtComponent);

            
            if(data.curShield > 0) effectManager.PlayAuraAnimation(Color.cyan);
            else if(data.curHp > 0) effectManager.PlayAuraAnimation(Color.red);

        }

        private void Update()
        {
            statusUI.Update();
            _animController.Update();
            _fsm.Update();
            data.playerMove = data.inputMove;

            float yaw = transform.eulerAngles.y;
            Vector3 forward = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, yaw, 0) * Vector3.right;
            Vector3 move =
                forward * data.playerMove.y +
                right   * data.playerMove.x;

            characterController.Move(
                move.normalized * (moveSpeed * Time.deltaTime)
            );
        }

        public void UpdateHp(float val)
        {
            data.curShield -= val;
            if (data.curShield < 0)
            {
                data.curHp += data.curShield;
                data.curShield = 0;
            }
            statusUI.SyncValues(data.curHp, data.maxHp, data.curShield, data.maxShield);
        }
        
        public void CreateDeathSnapshot()
        {
            GameObject root = SnapshotObjectPool.Instance.GetObj();
            root.transform.SetPositionAndRotation(transform.position, transform.rotation);
            var smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
            int index = 0;

            foreach (var smr in smrs)
            {
                if (index >= 10) break;

                Transform child = root.transform.GetChild(index);
                child.SetPositionAndRotation(smr.transform.position, smr.transform.rotation);
                child.localScale = smr.transform.lossyScale;

                var mf = child.GetComponent<MeshFilter>();
                var mr = child.GetComponent<MeshRenderer>();

                if (!mf.sharedMesh) mf.sharedMesh = new Mesh();
                mf.sharedMesh.indexFormat = IndexFormat.UInt32;
                smr.BakeMesh(mf.sharedMesh);
                Material[] mats = new Material[mf.sharedMesh.subMeshCount];
                for (int i = 0; i < mats.Length; i++) mats[i] = deathSnapshotMaterial;
                
                mr.sharedMaterials = mats;
                child.gameObject.SetActive(true);   
                index++;
            }

            // 多余子节点关闭
            for (int i = index; i < 10; i++)        
                root.transform.GetChild(i).gameObject.SetActive(false);

        }
    }
}