using System.Collections.Generic;
using Connect;
using Connect.Converter;
using Connect.Data;
using Connect.Prediction;
using Connect.Simulation;
using GameFSM.LevelFSM.LocomotionFSM;
using GameFSM.LevelFSM.PlayerFSM;
using mController.Player.InputController;
using mEvent;
using mModel;
using mSystem.InputSystem;
using mSystem.InputSystem.Context;
using mView.BulletChose;
using Player.Module;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using MessageType = Connect.Protocol.MessageType;
using Object = UnityEngine.Object;

namespace Player.ManagerLogic
{
    public class LocalPlayerLogic : IPlayerLogic, IController
    {
        private PlayerManager _m;
        private Vector3 _bottom;
        private float _stableTimer; // 输入状态切换当前时间

        private FsmContext _fsmContext;
        private PlayerInputController _inputController;
        private IUnRegister _mSetHandEvent;
        private BagModel _bagModel;
        
        private InputContext _context;
        private IInputSystem _inputSystem;

        private float _moveTimer;
        private float _boneTimer;
        
        private AnnulusView _annulusView; // 轮盘选择
        private float _timer;

        private uint _tick;
        private uint _sequence;
        private List<InputDataConverter.InputSample> _pendingSamples = new();
        public ushort buttons;
        
        // 运动状态机
        private LocomotionFsm _locomotionFsm;
        // 客户端状态循环数组
        private PredictionBuffer _predictionBuffer;
        
        public void Init(PlayerManager manager)
        {
            Debug.Log("创建本地玩家");
            _m = manager;
            _bagModel = this.GetModel<BagModel>();
            _context = new InputContext();
            _inputSystem = this.GetSystem<IInputSystem>();
            _inputSystem.InitHandler(_context);
            
            manager.CombatContext.InputContext = _context;
            manager.CombatModule = new PlayerCombatModule(manager.CombatContext);
            manager.MoveContext.CombatModule = manager.CombatModule;
            manager.MoveModule = new PlayerMoveModule(manager.MoveContext);
            manager.ViewModule = new PlayerViewModule(manager.PlayerContext);
            manager.BagModule = new PlayerBagModule(manager.PlayerContext);
            manager.IkModule = new PlayerIkModule(manager.IKContext);
            
            _fsmContext = new FsmContext
            {
                Locomotion = new Locomotion()
            };
            _inputController = new PlayerInputController(_m, _fsmContext);
            _locomotionFsm = new LocomotionFsm(_fsmContext.Locomotion, new LocomotionFsmCondition(_fsmContext.Locomotion, "Locomotion"));
            _predictionBuffer = new PredictionBuffer();
            if (NetworkService.Bridge is LocalServerSimulator lss)
            {
                lss.InitPlayerManager(manager);
            }
            _mSetHandEvent = this.RegisterEvent<SetHandIkTargetEvent>(InitHandIK);
            
            manager.BagModule.Init();
            manager.CombatModule.Init();
            manager.MoveModule.Init();
            manager.IkModule.Init();
            manager.ViewModule.Init();
            manager.MoveModule.InitFsmContext(_fsmContext);
            
            _annulusView = Object.FindObjectOfType<AnnulusView>(true); // 场景里找        
            if (_annulusView)
            {
                int count = _bagModel.BulletCount;
                _annulusView.Init(count, 1);
                for (int i = 0; i < count; i++)
                {
                    var bullet = _bagModel.GetBullet(i);
                    var img = _annulusView.icons[i].GetComponent<Image>();
                    if (img && ElementModel.Instance) img.sprite = ElementModel.Instance.elementUI[(int)bullet.Data];  
                }
                _annulusView.HideWheel();
            }

            //_syncManager = new PlayerSyncManager();
            //_syncManager.Register(new MoveSyncChannel(_m.EntityId));
            //_syncManager.Register(new AnimSyncChannel(_m.EntityId, _bagModel));
        }

        

        public void OnUpdate()
        {
            if (_inputSystem.GamePlayEnable) _inputController.Process(_context);
            else _m.MoveModule.InputMove(Vector2.zero);
            
            _m.state.aimPosition = _m.cameraController.rayEnd;

            _m.AnimController.Update();
            _m.IkModule?.OnUpdate();
            _m.ViewModule?.OnUpdate();
        }

        private InputDataConverter.InputSample SampleInput()
        {
            _tick++;
            Vector2 move = _fsmContext.Locomotion.InputMoveVec;

            InputDataConverter.InputSample sample = new InputDataConverter.InputSample
            {
                Tick = _tick,
                InputX = (byte)Mathf.RoundToInt((move.x + 1f) * 127f),
                InputY = (byte)Mathf.RoundToInt((move.y + 1f) * 127f),
                Yaw = (short)(_m.cameraController.Yaw * 10),
                Pitch = (short)(_m.cameraController.Pitch * 10),
                HoldButton = buttons
            };
            
            _pendingSamples.Add(sample);

            buttons = 0;

            return sample;
        }

        private void SendInputPacket()
        {
            if (_pendingSamples.Count == 0) return;
            _sequence++;

            byte[] data = InputDataConverter.Pack(
                _sequence,
                _pendingSamples.ToArray()
            );
            
            NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_PlayerInput, data));
            
            _pendingSamples.Clear();
        }
        
        private bool _prevSprint;
        private bool _prevJump;
        private bool _prevCrouch;

        public void OnClient50HzTick()
        {
            bool sprintHeld = _context.Sprint.Press;      
            bool jumpHeld   = _context.Jump.Press;        
            bool crouchHeld = _context.Crouch.Press;      

            _fsmContext.Locomotion.SprintDown = sprintHeld && !_prevSprint;
            _fsmContext.Locomotion.JumpDown   = jumpHeld && !_prevJump;
            _fsmContext.Locomotion.CrouchDown = crouchHeld && !_prevCrouch;
            _fsmContext.Locomotion.Yaw = _m.cameraController.Yaw;

            _prevSprint = sprintHeld;
            _prevJump   = jumpHeld;
            _prevCrouch = crouchHeld;
            
            InputDataConverter.InputSample sample = SampleInput(); // 50hz 采样
            
            _locomotionFsm.Update(); // 移动状态机更新
            _m.MoveModule.OnFixedUpdate();

            _predictionBuffer.Save(_tick, _m.transform.position, _fsmContext.Locomotion, sample);
            
            _m.state.position = _m.transform.position;  
            _m.state.rotation = _m.transform.rotation;  
            _m.MoveModule.WriteToState(_m.state);       
            _m.CombatModule.WriteToState(_m.state);
            
            Reload();
            
            _m.CombatModule.OnFixedUpdate();
        }

        public void OnClient20HzTick()
        {
            SendInputPacket();
        }

        private void Reload()
        {
            if (_context.Reload.Down)
            {
                _timer = 0;
            }

            if (_context.Reload.Press)
            {
                _timer += Time.deltaTime;
                if (_timer > 0.2f && !_annulusView.gameObject.activeSelf)
                {
                    _annulusView.ShowWheel();
                }

                if (_annulusView.gameObject.activeSelf)
                {
                    int idx = _annulusView.GetIndexByMouse(Input.mousePosition);
                    _annulusView.SetHighLight(idx);
                    if (idx >= 0)
                    {
                        _m.BagModule.SetChose(idx);
                    } 
                }
            }
            if (_context.Reload.Up)
            {
                _annulusView.HideWheel();
            }
        }

        public void OnLateUpdate()
        {
            _m.cameraController.UpdateForward();
        }

        private void InitHandIK(SetHandIkTargetEvent e)
        {
            _m.IkModule.SetHandIk(e.Trans);
            _m.IkModule.SetMuzzle(e.Muzzle);
        }

        public void UnRegister()
        {
            _mSetHandEvent.UnRegister();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }

        public void OnServerStateReceived(PlayerStateData data)
        {
            uint serverTick = data.lastProcessedTick;

            // 缓存已过期，丢弃
            if (!_predictionBuffer.TryGet(serverTick, out TickRecord record)) return;

            // 比对误差
            Vector3 offset = data.position - record.Position;
            if (offset.sqrMagnitude < 0.01f) return;
            Debug.Log($"预测失败 tick{serverTick}, offset={offset.magnitude:F3}");
            
            RelayForm(data);
        }

        private void RelayForm(PlayerStateData data)
        {
            uint serverTick = data.lastProcessedTick;
            if (!_predictionBuffer.TryGet(serverTick, out TickRecord baseRec) || !baseRec.HasData) return;
            
            // 先恢复到服务端 Tick N
            var loco = _fsmContext.Locomotion;     
            loco.IsGround      = data.isGround;    
            loco.IsJump        = data.isJump;      
            loco.IsSprint      = data.isSprint;    
            loco.IsCrouch      = data.isCrouch;    
            loco.LevelSpeed    = data.levelSpeed;  
            loco.VerticalSpeed = data.verticalSpeed;
            loco.Yaw           = data.yaw;
            loco.Pitch         = data.pitch;       
            loco.InputMoveVec  = data.inputMove;   
            loco.IsInputMove   = data.inputMove.sqrMagnitude >= 0.05f;      
            loco.IsMoveForward = data.inputMove.y >= 0.05f;

            _m.characterController.enabled = false;
            _m.transform.position = data.position;
            _m.characterController.enabled = true;

            for (uint t = serverTick + 1; t <= _tick; t++)
            {
                if (!_predictionBuffer.TryGet(t, out TickRecord rec) || !rec.HasData) continue;
                var input = rec.Input;

                loco.InputMoveVec = new Vector2(input.InputX / 127f - 1f, input.InputY / 127f - 1f);
                loco.IsInputMove = loco.InputMoveVec.sqrMagnitude >= 0.05f;
                loco.IsMoveForward = loco.InputMoveVec.y >= 0.05f;
                loco.Yaw = input.Yaw / 10f;

                loco.SprintDown = (input.HoldButton & PlayerInputStateBoolBit.BUTTON_SPRINT) != 0;
                loco.JumpDown = (input.HoldButton & PlayerInputStateBoolBit.BUTTON_JUMP) != 0;
                loco.CrouchDown = (input.HoldButton & PlayerInputStateBoolBit.BUTTON_CROUCH) != 0;
                
                _locomotionFsm.Update();
                _m.MoveModule.OnFixedUpdate();
            }
        }
    }
}