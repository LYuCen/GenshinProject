using Connect.Data;
using Connect.FlatDTO;
using Enemy;
using mController;
using mInterface;
using mSystem.HurtSystem;
using mSystem.StatusSystem;
using Player.AnimPro;
using Player.AnimTree;
using Player.ManagerLogic;
using QFramework;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameFSM.LevelFSM.PlayerFSM
{
    public class PlayerManager : MonoBehaviour, ICanRegisterEvent, IEntityIdentity
    {
        public PlayerStateData state = new PlayerStateData();
        public int EntityId => state.playerId;
        public EntityStatusUI EntityStatusUI => entityStatusUI;
        [Header("当前状态")] public string stateName;
        [Header("上半身状态")] public string upperName;
        public bool IsGround { get => state.isGround; set => state.isGround = value; }
        public bool IsJump { get => state.isJump; set => state.isJump = value; }
        public bool IsShoot { get => state.isShoot; set => state.isShoot = value; }
        public bool IsReload { get => state.isReload; set => state.isReload = value; }
        public bool IsSprint { get => state.isSprint; set => state.isSprint = value; }
        public bool IsThrow { get => state.isThrow; set => state.isThrow = value; }
        public bool IsCrouch { get => state.isCrouch; set => state.isCrouch = value; }
        public bool IsSliding { get => state.isSliding; set => state.isSliding = value; }
        public Vector2 InputMove { get => state.inputMove; set => state.inputMove = value; }
        public Vector2 PlayerMove { get => state.playerMove; set => state.playerMove = value; }
        [Header("角色骨骼")] public Transform[] bones;
        [Header("移动速度")] public float speed;
        [Header("脚下判定框高度")] public float h;
        [Header("判定半径")] public float radius;
        [Header("滑铲cd")] public float slidingCd;
        [Header("枪")] public Transform playerGun;
        [Header("目标")] public Transform aim;
        
        [Header("组件引用")]
        public Material pureMaterial;
        public CharacterController characterController;
        public Animator animator;
        public CameraController cameraController;
        public FirearmController firearmController;
        public PlayerAnimController AnimController;
        public EntityStatusUI entityStatusUI;
        public AimIK aimIK;
        public Transform gunCube, hand, gunStock;
        public AnimGraphData graphData;
        public CharacterEffectManager characterEffectManager;
        public FullBodyBipedIK bodyIk;
        
        private IPlayerLogic _logic;
        public bool isSlidingThrow; // 滑铲时投掷手雷
        public bool isShoulder; // 是否肩射
        public int aimIkWeight;
        public float rHeadIkWeight; // 左手 ik 权重
        
        public void Init(bool isLocal)
        {
            state.bRots = bones;
            animator.applyRootMotion = false;
            AnimController = new PlayerAnimController(this, graphData);
            firearmController.Init();
            if (isLocal) _logic = new LocalPlayerLogic();
            else _logic = new RemotePlayerLogic();
            
            playerGun = firearmController.GetWeapon(0);
            entityStatusUI.InitStatus(ref state);
            _logic.Init(this);
        }

        public void ResetGun()
        {
            playerGun.SetParent(gunCube, true); // 保持世界位置/旋转/缩放
            firearmController.Weapon.InitTransform();
        }

        public void SetParent(Transform child, Transform parent)
        {
            if (child.parent == parent) return;
            // 记录子物体的世界缩放
            Vector3 worldScale = child.lossyScale;

            // 设置父物体
            child.SetParent(parent);

            // 直接用保持世界缩放的方法，不需要手动除
            child.localScale = new Vector3(
                worldScale.x / parent.lossyScale.x,
                worldScale.y / parent.lossyScale.y,
                worldScale.z / parent.lossyScale.z
            );
        }
        
        public void SyncStatus(PlayerDataFlat dto)
        {
            // FlatBuffers 生成的属性是 PascalCase（首字母大写）
            state.curHp = dto.CurHp;
            state.maxHp = dto.MaxHp;
            state.curShield = dto.CurShield;
            state.maxShield = dto.MaxShield;

            // 2. 更新 UI 表现
            if (entityStatusUI)
            {
                entityStatusUI.SyncValues(dto.CurHp, dto.MaxHp, dto.CurShield, dto.MaxShield);
                
            }
            if (state.curHp == 0 && gameObject.activeSelf)
            {
                Debug.Log($"[PlayerManager] {state.playerId} 玩家死亡");
                gameObject.SetActive(false);
                CreateSnapshot();
            }
        }

        public void HurtHp(HurtComponent component)
        {
            float hurt = component.Harm;
            if(state.curShield > 0) characterEffectManager.PlayAuraAnimation(Color.cyan);
            else if(state.curHp > 0) characterEffectManager.PlayAuraAnimation(Color.red);
            if (hurt <= state.curShield)
            {
                state.curShield = Mathf.Clamp(state.curShield - hurt, 0, state.maxShield);
                
            }
            else
            {
                hurt -= state.curShield;
                state.curShield = 0;
                state.curHp = Mathf.Clamp(state.curHp - hurt, 0, state.maxHp);
                
            }
            
        }

        public void ReloadEnd()
        {
            IsReload = false;
            firearmController.Reload();
        }

        private void FixedUpdate() => _logic?.OnFixedUpdate();

        private void Update()
        { 
            _logic?.OnUpdate();
            aimIK.solver.IKPositionWeight = Mathf.MoveTowards(
                aimIK.solver.IKPositionWeight, 
                aimIkWeight, 
                7 * Time.deltaTime
            );
            bodyIk.solver.IKPositionWeight = Mathf.MoveTowards(
                bodyIk.solver.IKPositionWeight, 
                rHeadIkWeight, 
                7 * Time.deltaTime
            );
        }
        
        private void LateUpdate() => _logic?.OnLateUpdate();
        
        public IPlayerLogic GetLogic() => _logic;

        private void OnDisable()
        {
            AnimController.Destroy();
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

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}