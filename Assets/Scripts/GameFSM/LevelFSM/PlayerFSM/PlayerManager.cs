using Connect.Data;
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

namespace GameFSM.LevelFSM.PlayerFSM
{
    public class PlayerManager : MonoBehaviour, ICanRegisterEvent, IEntityIdentity
    {
        public PlayerStateData state = new PlayerStateData();
        public PlayerMoveQuery Query;
        public int EntityId => state.playerId;
        public EntityStatusUI EntityStatusUI => entityStatusUI;
        [Header("角色骨骼")] public Transform[] bones;
        
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
            state.bRots = bones;
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


        private void OnDestroy()
        {
            AnimController.Destroy();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}