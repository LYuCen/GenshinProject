using Connect.Data;
using mController;
using mEnum.Query;
using mInterface;
using mSystem.InputSystem.Context;
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
        public FirearmController firearmController;
        public PlayerAnimController AnimController;
        public EntityStatusUI entityStatusUI;
        public AimIK aimIK;
        public Transform gunCube, hand, gunStock;
        public AnimGraphData graphData;
        public CharacterEffectManager characterEffectManager;
        public FullBodyBipedIK bodyIk;
        
        private IPlayerLogic _logic;
        private InputContext _context;
        private PlayerContext _playerContext;
        private CombatContext _combatContext;
        private MoveContext _moveContext;
        private PlayerIkContext _ikContext;
        public PlayerViewModule ViewModule;
        public PlayerBagModule BagModule;
        public PlayerCombatModule CombatModule;
        public PlayerHpModule HpModule;
        public PlayerMoveModule MoveModule;
        public PlayerIkModule IkModule;
        
        public void Init(bool isLocal, InputContext context = null)
        {
            _context = context;
            state.bRots = bones;
            animator.applyRootMotion = false;
            AnimController = new PlayerAnimController(this, graphData);
            _playerContext = new PlayerContext
            {
                StateData = state,
            };
            _combatContext = new CombatContext
            {
                FirearmController = firearmController,
                InputContext = context,
                GunCube = gunCube,
            };
            CombatModule = new PlayerCombatModule(_combatContext);
            _moveContext = new MoveContext
            {
                CameraController = cameraController,
                CharacterController = characterController,
                PlayerTransform = transform,
                CombatModule = CombatModule
            };
            _ikContext = new PlayerIkContext
            {
                CameraController = cameraController,
                AimIK = aimIK,
                BodyBipedIK = bodyIk
            };
            MoveModule = new PlayerMoveModule(_moveContext);
            ViewModule = new PlayerViewModule(_playerContext);
            BagModule = new PlayerBagModule(_playerContext);
            IkModule = new PlayerIkModule(_ikContext);
            HpModule = new PlayerHpModule(_playerContext);
            firearmController.Init(_playerContext);
            if (isLocal) _logic = new LocalPlayerLogic();
            else _logic = new RemotePlayerLogic();
            
                                                                                               
            entityStatusUI.InitStatus(ref state);
            _logic.Init(this, _context);
            BagModule.Init();
            CombatModule.Init();
            MoveModule.Init();
            IkModule.Init();
            ViewModule.Init();
        }

        private void FixedUpdate()
        { 
            _logic?.OnFixedUpdate();
        }

        private void Update()
        { 
            _logic?.OnUpdate();
            
            /*
            bodyIk.solver.IKPositionWeight = Mathf.MoveTowards(
                bodyIk.solver.IKPositionWeight, 
                rHeadIkWeight, 
                7 * Time.deltaTime
            );
            */
            
            ViewModule.OnUpdate();
            CombatModule.OnUpdate();
            MoveModule.OnUpdate();
            IkModule.OnUpdate();
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