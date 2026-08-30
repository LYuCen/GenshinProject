using System;
using System.Net.Sockets;
using AI;
using AI.VisualRequest.VisualRequest;
using BaseClass;
using Connect;
using Connect.Converter;
using Connect.Data;
using Connect.DTO;
using Connect.FlatDTO;
using Connect.HitRewind;
using Connect.Protocol;
using GameFSM.LevelFSM.LocomotionFSM;
using Google.FlatBuffers;
using mEnum;
using mInterface;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem;
using mSystem.HurtSystem;
using mSystem.InputSystem;
using mSystem.StatusSystem;
using mWeapon;
using mWeapon.Component;
using mWeapon.WeaponFeature;
using Player.AnimPro;
using Player.AnimTree;
using UnityEngine;
using ResourceManager = AssetBundleFramework.Core.Resource.ResourceManager;

namespace Server
{
    public class ServerPlayerObject : MonoBehaviour, IEntityIdentity, IManager
    {
        public int EntityId => playerId;
        public EntityStatusUI EntityStatusUI => null;
        public int playerId;
        public Socket ClientSocket;
        public PlayerStateData State = new PlayerStateData();
        public HitColliderEntry[] hitColliders = new HitColliderEntry[11];
        private HitHistoryBuffer _hitHistory;
        private HitShape[] _currentShapes;

        private IElementBuffSystem _buffSystem;
        private ElementBuffController _buffController;

        public Animator animator;
        public AnimGraphData graphData;
        private ServerAnimContext _animContext;
        private ServerCombatProvider _combatProvider;
        private PlayerAnimController _animController;

        private ServerMoveModule _moveModule;
        [SerializeField] private CharacterController characterController;
        private PlayerInputBuffer _inputBuffer = new();

        private bool _prevJump;
        private bool _prevCrouch;
        private bool _prevSprint;
        private bool _prevShoot, _prevAds, _prevReload; 

        private Locomotion _locomotion;
        private LocomotionFsm _locomotionFsm;

        private WeaponPro _weapon;
        private uint _currentInputServerTick; // 当前处理的输入对应的服务端时间
        [SerializeField] private Transform fireOrigin; // 开火起点位置
        
        // 摄像机模拟
        private const float CamRayLength      = 100f;
        private const float CamDistance       = 2f;     
        private const float CamShoulderOffset = 0.2f;   
        private const float CamHeightStand    = 1.5f;
        private const float CamHeightCrouch   = 1.2f; 

        private LineVisualRequest _shotLine; // 射击射线
        private PointVisualRequest _hitPoint; // 命中点
        private LayerMask _aimMask;
        private LayerMask _wallMask;
        public bool isDead;

        public void Init(int id, Socket s)
        {
            playerId = id;
            ClientSocket = s;
            State.playerId = id;
            _aimMask = LayerMask.GetMask("Default", "Hitbox", "Plane", "Wall"); 
            _wallMask =  _wallMask = LayerMask.GetMask("Wall", "Plane");
            
            State.maxHp = 200f;
            State.curHp = 200f;
            State.maxShield = 100f;
            State.curShield = 100f;
            _hitHistory = new HitHistoryBuffer(8);
            _currentShapes = new HitShape[11];
            _locomotion = new Locomotion();
            _moveModule = new ServerMoveModule(_locomotion, characterController);
            _locomotionFsm = new LocomotionFsm(_locomotion, new LocomotionFsmCondition(_locomotion, "Locomotion"));
            
            foreach (var entry in hitColliders)
            {
                if (entry.collider != null)
                    entry.collider.enabled = false;
            }
            
            _buffController = gameObject.GetComponent<ElementBuffController>();
            if (!_buffController) _buffController = gameObject.AddComponent<ElementBuffController>();
            _buffController.id = id;
            _buffController.TimeProvider = () => UnityServerManager.Instance.ServerTickCount * 0.02f;
            _buffSystem = new ElementBuffSystem();

            // 初始化服务端动画系统
            if (animator == null) animator = GetComponent<Animator>();
            if (animator != null && graphData != null)
            {
                var moveProvider = new ServerMoveProvider(State);
                _combatProvider = new ServerCombatProvider(State);
                Transform weaponPivot = transform;
                _animContext = new ServerAnimContext(moveProvider, _combatProvider, weaponPivot, animator);
                _animController = new PlayerAnimController(_animContext, graphData);
            }
            InitWeapon();
        }

        private void InitWeapon()
        {
            var ir = ResourceManager.instance.Load
                ("Assets/AssetBundle/ScriptableObject/Weapon/FnIws/FnIws.asset", false);
            var inv = new ServerInventory();
            inv.Seed(ElementEnum.Fire, 999);
            inv.Seed(ElementEnum.Elect, 999);
            inv.Seed(ElementEnum.Grass, 999);
            inv.Seed(ElementEnum.Water, 999);
            inv.Seed(ElementEnum.Frost, 999);
            _weapon = new WeaponPro(ir.GetAsset<WeaponData>(), inv);
            _weapon.RequireFeature<FireFeature>().OnFire += OnServerFire;

            _shotLine = new LineVisualRequest
            {
                Enable = false,
                LineColor = Color.red
            };
            _hitPoint = new PointVisualRequest
            {
                Enable = false,
                Radius = 0.15f,
                PointColor = Color.green
            };
            VisualizerManager.Instance.Register(_shotLine); 
            VisualizerManager.Instance.Register(_hitPoint); 
        }
        
        public static class ButtonInput
        {
            public static ButtonState Build(bool held,  
                ref bool prevHeld)
            {
                var s = new ButtonState
                {
                    Down = held && !prevHeld,
                    Press = held,
                    Up = !held && prevHeld
                };
                prevHeld = held;
                return s;
            }
        }

        private float _lastLogTime;
        public void ServerTick(float deltaTime)
        {
            if (Time.realtimeSinceStartup - _lastLogTime > 1f)
            {
                Debug.Log($"{playerId}:ReadyCount={_inputBuffer.ReadyCount}");
                _lastLogTime = Time.realtimeSinceStartup;
            }

            if (_inputBuffer.ReadyCount <= 2)
            {
                if (!_inputBuffer.TryDequeue(out var sample)) return;
                StepOnce(sample, deltaTime);
            }
            else if (_inputBuffer.ReadyCount <= 5)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (!_inputBuffer.TryDequeue(out var sample)) break;
                    StepOnce(sample, deltaTime);
                }
            }
            else if (_inputBuffer.ReadyCount <= 10)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!_inputBuffer.TryDequeue(out var sample)) break;
                    StepOnce(sample, deltaTime);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!_inputBuffer.TryDequeue(out var sample)) break;
                    StepOnce(sample, deltaTime);
                }
            }
        }

        private void StepOnce(InputDataConverter.InputSample sample, float deltaTime)
        {
            float yaw = sample.Yaw / 10f;

            Vector2 move = new Vector2(sample.InputX / 127f - 1f, sample.InputY / 127f - 1f);

            _locomotion.Yaw = yaw;
            _locomotion.Pitch = sample.Pitch / 10f;
            _locomotion.InputMoveVec = move;
            _locomotion.IsInputMove = move.sqrMagnitude >= 0.05f;
            _locomotion.IsMoveForward = move.y >= 0.05f;
            
            bool sprintHeld = (sample.HoldButton & PlayerInputStateBoolBit.BUTTON_SPRINT) != 0;      
            bool jumpHeld   = (sample.HoldButton & PlayerInputStateBoolBit.BUTTON_JUMP) != 0;        
            bool crouchHeld = (sample.HoldButton & PlayerInputStateBoolBit.BUTTON_CROUCH) != 0;      

            _locomotion.SprintDown = sprintHeld && !_prevSprint;
            _locomotion.JumpDown   = jumpHeld && !_prevJump;
            _locomotion.CrouchDown = crouchHeld && !_prevCrouch;

            _prevSprint = sprintHeld;
            _prevJump   = jumpHeld;
            _prevCrouch = crouchHeld;
            State.lastProcessedTick = sample.Tick;
            _locomotion.IsAiming = _weapon != null && _weapon.Runtime.Stance != WeaponStance.Normal;
            
            _locomotionFsm.Update();
            _moveModule.Update(deltaTime);
            
            State.isGround  = _locomotion.IsGround;       
            State.isJump    = _locomotion.IsJump;
            State.isSprint  = _locomotion.IsSprint;       
            State.isCrouch  = _locomotion.IsCrouch;       
            State.inputMove = _locomotion.InputMoveVec;
            State.levelSpeed = _locomotion.LevelSpeed;
            State.verticalSpeed = _locomotion.VerticalSpeed;
            State.yaw = _locomotion.Yaw;
            State.pitch = _locomotion.Pitch;
            State.position = transform.position;

            bool shootHeld = (sample.HoldButton & PlayerInputStateBoolBit.BUTTON_SHOOT) != 0;
            bool adsHeld = (sample.HoldButton & PlayerInputStateBoolBit.BUTTON_ADS) != 0;    
            bool reloadHeld = (sample.HoldButton & PlayerInputStateBoolBit.BUTTON_RELOAD) != 0;
            
            var action = new WeaponAction()
            {
                Primary = ButtonInput.Build(shootHeld, ref _prevShoot),
                Secondary = ButtonInput.Build(adsHeld, ref _prevAds),
                Reload = ButtonInput.Build(reloadHeld, ref _prevReload)
            };

            _currentInputServerTick = UnityServerManager.Instance.ServerTickCount;
            SimulateCameraAim();
            _weapon.HandleAction(action);
            _weapon.Tick(deltaTime);

            State.isReload = _weapon.Runtime.HasState(WeaponStateTags.Reloading);
            State.isShoot = _weapon.Runtime.Stance == WeaponStance.Ads;
            
            _animController.Update();
            CacheSnapshotFromColliders();
        }

        /// <summary>
        /// 摄像机模拟
        /// </summary>
        private void SimulateCameraAim()
        {
            float pitch = State.pitch;
            float yaw = State.yaw;
            float height = State.isCrouch ? CamHeightCrouch : CamHeightStand;
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 targetPos = State.position + Vector3.up * height;
            Vector3 camPos = targetPos - rot * Vector3.forward * CamDistance + 
                             rot * Vector3.right * CamShoulderOffset;

            State.aimPosition = GetAimPoint(GetCameraRay(), _currentInputServerTick);
        }

        private Vector3 GetAimPoint(Ray camRay, uint shootTick)
        {
            float bestT = CamRayLength;
            Vector3 aimPoint = camRay.GetPoint(CamRayLength);
            
            if (Physics.Raycast(camRay, out RaycastHit hit, CamRayLength, _aimMask))
            {
                if (hit.distance < bestT)
                {
                    bestT = hit.distance;
                    aimPoint = hit.point;
                }
            }
            
            foreach (var kv in UnityServerManager.Instance.AllPlayers)
            {
                if (kv.Key == playerId) continue;       
                var victim = kv.Value;
                if (!victim._hitHistory.FindClosest(shootTick, out var snap) || snap.Shapes == null) continue;     
                foreach (var shape in snap.Shapes)      
                {
                    float t = CapsuleRayMath.Intersect(camRay, shape);        
                    if (t > 0f && t < bestT)
                    {
                        bestT = t;
                        aimPoint = camRay.GetPoint(t);  
                    }
                }
            }

            return aimPoint;
        }

        private Ray GetCameraRay()
        {
            float pitch = State.pitch;
            float yaw = State.yaw;
            float height = State.isCrouch ? CamHeightCrouch : CamHeightStand;
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 targetPos = State.position + Vector3.up * height;
            Vector3 camPos = targetPos - rot * Vector3.forward * CamDistance + 
                             rot * Vector3.right * CamShoulderOffset;
            return new Ray(camPos, rot * Vector3.forward);
        }
        
        public void HandleBinaryMessage(MessageType type, byte[] data)
        {
            switch (type)
            {
                case MessageType.C2S_Revive:
                    isDead = false;
                    Vector3 revivePos = GetSpawnPosition();
                    characterController.enabled = false;
                    transform.position = revivePos;
                    characterController.enabled = true;

                    State.position = revivePos;
                    State.curHp = State.maxHp;
                    State.curShield = State.maxShield;
                    
                    
                    
                    byte[] reviveData = new byte[32];
                    int off = 0;
                    Buffer.BlockCopy(BitConverter.GetBytes(playerId), 0, reviveData, off, 4); off += 4;       
                    Buffer.BlockCopy(BitConverter.GetBytes(revivePos.x), 0, reviveData, off, 4); off += 4;    
                    Buffer.BlockCopy(BitConverter.GetBytes(revivePos.y), 0, reviveData, off, 4); off += 4;    
                    Buffer.BlockCopy(BitConverter.GetBytes(revivePos.z), 0, reviveData, off, 4); off += 4;    
                    Buffer.BlockCopy(BitConverter.GetBytes(State.curHp), 0, reviveData, off, 4); off += 4;    
                    Buffer.BlockCopy(BitConverter.GetBytes(State.curShield), 0, reviveData, off, 4); off += 4;
                    Buffer.BlockCopy(BitConverter.GetBytes(State.maxHp), 0, reviveData, off, 4); off += 4;    
                    Buffer.BlockCopy(BitConverter.GetBytes(State.maxShield), 0, reviveData, off, 4);

                    var reviveMsg = new Message(MessageType.S2C_Revive, reviveData);   
                    UnityServerManager.Instance.Broadcast(reviveMsg);
                    break;
                
                case MessageType.C2S_PlayerInput:
                    _inputBuffer.Enqueue(InputDataConverter.Unpack(data));
                    break;
                
                case MessageType.C2S_ChooseElement:
                    if (data.Length >= 1)
                    {
                        (_weapon.Inventory as ServerInventory)?.SetChose(data[0]);
                    }
                    break;
            }
        }
        
        // 获取当前玩家的 Offset (用于全量广播构建列表)
        public Offset<PlayerDataFlat> GetCurrentOffset(FlatBufferBuilder builder)
        {
            // 注意：这里调用你之前在模拟器里写的那个 CreateSinglePlayerOffset 方法
            // 同步物理引擎位置到 State
            State.position = transform.position;
            State.rotation = transform.rotation;
    
            return PlayerDataConverter.CreateSinglePlayerOffset(builder, State);
        }

        private void CacheSnapshotFromColliders()
        {
            HitShapeExtractor.Extract(hitColliders, _currentShapes);
            _hitHistory.Add(new HitSnapshot
            {
                ServerTick = UnityServerManager.Instance.ServerTickCount,
                Shapes = _currentShapes
            });
        }
        

        private void RewindHitCheck(ShootData shoot)
        {
            Vector3 origin = shoot.position;
            Vector3 forward = shoot.forward;
            float wallDist = 100f;
            
            Ray ray = new Ray(origin, forward);
            if (Physics.Raycast(ray, out RaycastHit wallHit, 100f, _wallMask))
            {
                wallDist = wallHit.distance;
            }

            uint shootClientTick = shoot.clientTick;

            float closestDist = float.MaxValue;
            int bestVictimId = -1;
            HitPart bestPart = HitPart.Body;
            int shooterTeam = UnityServerManager.Instance.GetTeam(shoot.shooterId);

            foreach (var kv in UnityServerManager.Instance.AllPlayers)
            {
                int victimId = kv.Key;
                if (victimId == shoot.shooterId) continue;
                // 关闭友伤
                if(shooterTeam >= 0 && UnityServerManager.Instance.GetTeam(victimId) == shooterTeam) continue;
                var victim = kv.Value;

                // 粗筛：包围球
                if (!CapsuleRayMath.RayBoundingSphere(origin, forward, victim.State.position, 2f))
                    continue;

                // 取最接近射击时刻的快照
                if (!victim._hitHistory.FindClosest(shootClientTick, out var snap)) continue;
                if (snap.Shapes == null) continue;

                // 遍历受害者所有判定框
                foreach (var shape in snap.Shapes)
                {
                    float t = CapsuleRayMath.Intersect(ray, shape);
                    if (t > 0f && t < closestDist && t < wallDist)
                    {
                        closestDist = t;
                        bestVictimId = victimId;
                        bestPart = shape.Part;
                    }
                }
            }

            if (bestVictimId == -1) return;
            // 绘制射击射线
            Vector3 hitPos = origin + forward * closestDist;
            _shotLine.EndPos = hitPos;
            _hitPoint.PointPos = hitPos;
            _hitPoint.Text = $"hit {bestVictimId} {bestPart}";
            _hitPoint.Enable = true;

            // 应用伤害
            int damage = bestPart == HitPart.Head ? (int)shoot.headHit : (int)shoot.baseHit;
            var target = UnityServerManager.Instance.GetPlayer(bestVictimId);

            HurtComponent component = new HurtComponent
            {
                attackerId = shoot.shooterId,
                Element = shoot.element,
                ElementMeasure = shoot.elementMeasure,
                Harm = damage,
                HitType = (int)bestPart,
                Transform = target.transform,
                victimId = bestVictimId
            };

            HurtComponentSystem.ProcessElementReaction(component, target._buffController);
            
            HitDto dto = new HitDto()
            {
                attackerId = shoot.shooterId,
                victimId = bestVictimId,
                damage = component.Harm,
                element = shoot.element,
                elementMeasure = shoot.elementMeasure,
                hitType = (int)bestPart
            };

            UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_DamageConfirm,
                HitDataConverter.ToBinary(dto)));
            component.SkipReaction = true;
            HurtSystem.Instance.EnQueue(component);

            UnityServerManager.Instance.SetLastAttacker(bestVictimId, shoot.shooterId);
        }

        private void OnServerFire()
        {
            var fire = _weapon.RequireFeature<FireFeature>();
            var damageProfile = _weapon.RequireComponent<DamageProfileComponent>();
            var mag = _weapon.RequireComponent<MagazineComponent>();

            Vector3 origin = fireOrigin.position;
            Vector3 aimPoint = GetAimPoint(GetCameraRay(), _currentInputServerTick);
            Vector3 forward = (aimPoint - origin).normalized;
                
            var data = new ShootData
            {
                shooterId      = playerId,
                position       = origin,
                forward        = forward,
                speed          = fire.Data.muzzleVelocity,
                baseHit        = fire.Data.damage * damageProfile.BodyMultiplier,
                headHit        = fire.Data.damage * damageProfile.HeadMultiplier,
                element        = (int)mag.ElementEnum,  
                elementMeasure = fire.Data.measure,     
                moveTime       = 3f,
                clientTick     = _currentInputServerTick
            };

            _shotLine.StartPos = data.position;
            _shotLine.EndPos = data.position + data.forward * 50f;
            _shotLine.Enable = true;
            _hitPoint.Enable = false;
            
            // 广播射击
            UnityServerManager.Instance.BroadcastExcept(new Message(MessageType.S2C_PlayerShoot, ShootDataConverter.ToDto(data)), playerId); 
            RewindHitCheck(data);
        }
        

        /// <summary>
        ///  复活点选择
        /// </summary>
        private Vector3 GetSpawnPosition()
        {
            return UnityServerManager.Instance.GetRebirthPosition(playerId);
        }
        
        public void UpdateHp(float val)
        {
            if (isDead) return;
            State.curShield -= val;
            if (State.curShield < 0)
            {
                State.curHp += State.curShield;   
                State.curShield = 0;
            }

            var hpMsg = PlayerDataConverter.PackHp(playerId, State.curHp, State.curShield, State.maxHp, State.maxShield);

            UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_Hp, hpMsg));

            if (State.curHp <= 0)
            {
                State.curHp = 0;
                State.curShield = 0;
                isDead = true;
                byte[] deathData = BitConverter.GetBytes(playerId);
                UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_PlayerDeath, deathData));
                UnityServerManager.Instance.OnPlayerDeath(playerId);
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (hitColliders == null) return;

            foreach (var entry in hitColliders)
            {
                if (entry.collider == null) continue;

                Gizmos.color = GetPartColor(entry.hitPart);
                Transform t = entry.collider.transform;

                if (entry.collider is CapsuleCollider cc) DrawCapsuleGizmo(t, cc);
                else if (entry.collider is SphereCollider sc) DrawSphereGizmo(t, sc);
                else if (entry.collider is BoxCollider bc) DrawBoxGizmo(t, bc);
            }
        }

        static Color GetPartColor(HitPart part)
        {
            return part switch
            {
                HitPart.Head => Color.red,
                HitPart.Body => Color.cyan,
                HitPart.Arm  => Color.green,
                HitPart.Leg  => Color.green,
                _            => Color.gray
            };
        }

        static void DrawBoxGizmo(Transform t, BoxCollider bc)
        {
            Gizmos.matrix = Matrix4x4.TRS(
                t.TransformPoint(bc.center),
                t.rotation,
                Vector3.Scale(bc.size, t.lossyScale));
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity;
        }

        static void DrawSphereGizmo(Transform t, SphereCollider sc)
        {
            float r = sc.radius * Mathf.Max(t.lossyScale.x, Mathf.Max(t.lossyScale.y, t.lossyScale.z));
            Gizmos.DrawWireSphere(t.TransformPoint(sc.center), r);
        }

        static void DrawCapsuleGizmo(Transform t, CapsuleCollider cc)
        {
            float scale = t.lossyScale.y;
            float halfH = Mathf.Max(0, cc.height * 0.5f - cc.radius) * scale;
            float r = cc.radius * Mathf.Max(t.lossyScale.x, t.lossyScale.z);

            Vector3 worldCenter = t.TransformPoint(cc.center);
            Vector3 axis = cc.direction switch
            {
                0 => t.right,
                1 => t.up,
                2 => t.forward,
                _ => t.up
            };

            Vector3 top = worldCenter + axis * halfH;
            Vector3 bottom = worldCenter - axis * halfH;

            // 两端球体
            Gizmos.DrawWireSphere(top, r);
            Gizmos.DrawWireSphere(bottom, r);

            // 圆柱体连接线
            Vector3 perp = Vector3.Cross(axis, t.forward).normalized;
            if (perp.sqrMagnitude < 0.01f) perp = Vector3.Cross(axis, t.right).normalized;
            Vector3 perp2 = Vector3.Cross(axis, perp).normalized;

            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 0.25f;
                Vector3 offset = (Mathf.Cos(angle) * perp + Mathf.Sin(angle) * perp2) * r;
                Gizmos.DrawLine(bottom + offset, top + offset);
            }
        }
#endif
    }
}