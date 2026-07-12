using System;
using Connect;
using Connect.Data;
using Connect.Protocol;
using mController;
using mEnum.Flags;
using mEnum.Query;
using mInterface;
using mSystem.StatusSystem;
using Player;
using Player.AnimPro;
using Player.AnimTree;
using Player.Context;
using Player.ManagerLogic;
using Player.Module;
using QFramework;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameFSM.LevelFSM.PlayerFSM
{
    public class PlayerManager : MonoBehaviour, ICanRegisterEvent, IEntityIdentity, IAnimContext
    {
        public PlayerStateData state = new PlayerStateData();
        public PlayerMoveQuery Query;
        public int EntityId => state.playerId;
        public EntityStatusUI EntityStatusUI => entityStatusUI;
        [Header("死亡材质")]  public Material deathSnapshotMaterial;
        
        [Header("组件引用")]
        public CharacterController characterController;
        public Animator animator;
        public CameraController cameraController;
        public PlayerAnimController AnimController;
        public EntityStatusUI entityStatusUI;
        public AimIK aimIK;
        public Transform gunCube, hand, gunStock;
        public AnimGraphData graphData;
        public CharacterEffectManager characterEffectManager;
        public FullBodyBipedIK bodyIk;
        public Transform weaponPivot;
        
        private IPlayerLogic _logic;
        public PlayerContext PlayerContext;
        public CombatContext CombatContext;
        public MoveContext MoveContext;
        public PlayerIkContext IKContext;
        public PlayerHpContext HpContext;
        public PlayerViewModule ViewModule;
        public PlayerBagModule BagModule;
        public PlayerCombatModule CombatModule;
        public PlayerHpModule HpModule;
        public PlayerMoveModule MoveModule;
        public PlayerIkModule IkModule;
        private bool _isLocal;
        public bool IsLocal => _isLocal;
        
        public void Init(bool isLocal)
        {
            _isLocal = isLocal;
            animator.applyRootMotion = false;
            AnimController = new PlayerAnimController(this, graphData);
            PlayerContext = new PlayerContext
            {
                StateData = state,
                CharacterEffectManager = characterEffectManager,
            };

            HpContext = new PlayerHpContext()
            {
                StateData = state,
                CharacterEffectManager = characterEffectManager,
                EntityStatusUI = entityStatusUI
            };
            
            // 本地远程公共模块
            HpModule = new PlayerHpModule(HpContext);
            CombatContext = new CombatContext
            {
                CameraController = cameraController,
                GunCube = gunCube,
                AnimController = AnimController,
                IsLocal = isLocal
            };
            MoveContext = new MoveContext
            {
                CameraController = cameraController,
                CharacterController = characterController,
                PlayerTransform = transform,
            };
            IKContext = new PlayerIkContext
            {
                CameraController = cameraController,
                AimIK = aimIK,
                BodyBipedIK = bodyIk,
                IsLocal = isLocal
            };

            // 主控玩家模块
            if (isLocal)
            {
                _logic = new LocalPlayerLogic();
            }
            else
            {
                _logic = new RemotePlayerLogic();
            }
            _logic.Init(this);
            
            // 初始化
            entityStatusUI.InitStatus(ref state);
        }

        private void FixedUpdate()
        { 
            _logic?.OnFixedUpdate();
            HpModule?.OnFixedUpdate();
        }

        private void Update()
        { 
            _logic?.OnUpdate();
            HpModule?.OnUpdate();
        }

        private void LateUpdate()
        {
            _logic?.OnLateUpdate();
        }
        
        public IPlayerLogic GetLogic() => _logic;

        private void OnDisable()
        {
            if (_logic is LocalPlayerLogic logic)
            {
                logic.UnRegister();
            }
        }
        
        public float GetAimIkTarget()
        {
            PlayerMoveQuery move = MoveModule?.GetQuery() ?? default;
            if(move.HasState(PlayerMoveFlags.Sprint)) return 0;
            if(CombatModule?.GetQuery()?.HasState(WeaponFlags.Reload) == true) return 0;
            if(CombatModule?.GetQuery()?.HasState(WeaponFlags.Bolt) == true) return 0;
            if(CombatModule?.GetQuery()?.HasState(WeaponFlags.Throw) == true) return 0;

            return 1;
        }

        public float GetHandIkTarget()
        {
            PlayerMoveQuery move = MoveModule?.GetQuery() ?? default;
            if(move.HasState(PlayerMoveFlags.Sprint)) return 0;
            if(CombatModule?.GetQuery()?.HasState(WeaponFlags.Reload) == true) return 0;
            if(CombatModule?.GetQuery()?.HasState(WeaponFlags.Bolt) == true) return 0;
            if(CombatModule?.GetQuery()?.HasState(WeaponFlags.Throw) == true) return 0;

            return 1;
        }

        public void DisableModule()
        {
            CombatModule.SetEnable(false);
            MoveModule.SetEnable(false);
        }

        public void EnableModule()
        {
            CombatModule.Reset();
            MoveModule.Reset();
            IkModule.Reset();
            
            CombatModule.SetEnable(true);
            MoveModule.SetEnable(true);
        }

        public void DisableVisual()
        {
            foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>()) r.enabled = false;
            gunCube?.gameObject.SetActive(false);
            characterController.enabled = false;
        }

        public void EnableVisual()
        {
            foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>()) r.enabled = true;
            gunCube?.gameObject.SetActive(true);
            characterController.enabled = true;
        }

        public void SendReviveRequest()
        {
            var msg = new Message(MessageType.C2S_Revive, BitConverter.GetBytes(EntityId));
            NetworkService.Bridge.SendToServer(msg);
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
        
        private void OnDestroy()
        {
            AnimController.Destroy();
        }

        IMoveQueryProvider IAnimContext.MoveModule => MoveModule;
        ICombatQueryProvider IAnimContext.CombatModule => CombatModule;
        IIkProvider IAnimContext.IkModule => IkModule;
        Transform IAnimContext.WeaponPivot => weaponPivot;
        Animator IAnimContext.Animator => animator;

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}