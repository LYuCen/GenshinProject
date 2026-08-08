using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using BaseClass;
using Connect;
using Connect.AnimDTO;
using Connect.Converter;
using Connect.Data;
using Connect.DTO;
using Connect.FlatDTO;
using Connect.HitRewind;
using Connect.Protocol;
using Connect.ShootDTO;
using GameFSM.LevelFSM.LocomotionFSM;
using Google.FlatBuffers;
using mEnum;
using mEnum.Flags;
using mInterface;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem;
using mSystem.HurtSystem;
using mSystem.StatusSystem;
using Player.AnimPro;
using Player.AnimTree;
using UnityEngine;

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

        private Locomotion _locomotion;
        private LocomotionFsm _locomotionFsm;

        public void Init(int id, Socket s)
        {
            playerId = id;
            ClientSocket = s;
            State.playerId = id;
            
            State.maxHp = 200f;
            State.curHp = 200f;
            State.maxShield = 100f;
            State.curShield = 100f;
            _hitHistory = new HitHistoryBuffer(8);
            _currentShapes = new HitShape[11];
            _locomotion = new Locomotion();
            _moveModule = new ServerMoveModule(_locomotion, characterController);
            _locomotionFsm = new LocomotionFsm(_locomotion, new LocomotionFsmCondition(_locomotion, "Locomotion"));

            // 启动接收线程
            Thread t = new Thread(ReceiveLoop);
            t.IsBackground = true;
            t.Start();
            
            foreach (var entry in hitColliders)
            {
                if (entry.collider != null)
                    entry.collider.enabled = false;
            }
            
            _buffController = gameObject.GetComponent<ElementBuffController>();
            if (!_buffController) _buffController = gameObject.AddComponent<ElementBuffController>();
            _buffController.id = id;
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
        }

        public void ServerTick(float deltaTime)
        {
            if (_inputBuffer.TryDequeue(out var sample))
            {
                float yaw = sample.Yaw / 10f;

                Vector2 move = new Vector2(
                    sample.InputX / 127f - 1f,
                    sample.InputY / 127f - 1f
                );

                _locomotion.Yaw = yaw;
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
            }
            _locomotionFsm.Update();
            _moveModule.Update(deltaTime);
            
            State.isGround  = _locomotion.IsGround;       
            State.isJump    = _locomotion.IsJump;
            State.isSprint  = _locomotion.IsSprint;       
            State.isCrouch  = _locomotion.IsCrouch;       
            State.inputMove = _locomotion.InputMoveVec;  
            _animController.Update();
        }

        private void ReceiveLoop()
        {
            // 二进制下不再使用 StringBuilder
            while (ClientSocket != null && ClientSocket.Connected)
            {
                try
                {
                    byte[] headBuff = new byte[4];
                    if (!ReadExact(ClientSocket, headBuff, 4)) break;
                    int totalSize = BitConverter.ToInt32(headBuff, 0);
                    
                    byte[] bodyBuff = new byte[totalSize];
                    if (!ReadExact(ClientSocket, bodyBuff, totalSize)) break;
                    
                    MessageType type = (MessageType)BitConverter.ToUInt16(bodyBuff, 0);
                    int dataOffset = 2;
                    
                    byte[] flatData = new byte[totalSize - dataOffset];
                    Buffer.BlockCopy(bodyBuff, dataOffset, flatData, 0, flatData.Length);
                    
                    UnityServerManager.Instance.EnqueueTask(() => 
                    {
                        HandleBinaryMessage(type, flatData);
                    });
                }
                catch (Exception ex)
                {
                    Debug.Log($"玩家 {playerId} 接收异常: {ex.Message}");
                    break;
                }
            }
    
            UnityServerManager.Instance.EnqueueTask(() => {
                UnityServerManager.Instance.CloseClient(ClientSocket);
            });
        }

        // 辅助方法：确保读取完整字节
        private bool ReadExact(Socket s, byte[] data, int size)
        {
            int received = 0;
            while (received < size)
            {
                int count = s.Receive(data, received, size - received, SocketFlags.None);
                if (count <= 0) return false;
                received += count;
            }
            return true;
        }
        
        private void HandleBinaryMessage(MessageType type, byte[] data)
        {
            switch (type)
            {
                case MessageType.C2S_PlayerShoot:
                    Message shootMsg = new Message(MessageType.S2C_PlayerShoot, data);
                    UnityServerManager.Instance.BroadcastExcept(shootMsg,  playerId);
                    ByteBuffer bb = new ByteBuffer(data);
                    var shootFlat = ShootDataFlat.GetRootAsShootDataFlat(bb);
                    RewindHitCheck(shootFlat);
                    break;
                
                case MessageType.Both_Heartbeat:
                    UnityServerManager.Instance.UpdateHeartbeat(playerId);
                    float serverTime = Time.time;
                    float clientSendTime = BitConverter.ToSingle(data, 0);

                    float halfRtt = (serverTime - clientSendTime) * 0.5f;
                    float offset = serverTime - clientSendTime - halfRtt;
                    UnityServerManager.Instance.TimeOffset[playerId] = offset;
                    
                    
                    byte[] response = new byte[8];
                    Buffer.BlockCopy(data, 0, response, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(serverTime), 0, response, 4, 4);

                    // 回传客户端时间 + 服务端时间
                    
                    Message heartbeatAck = new Message(MessageType.Both_Heartbeat, response);
                    UnityServerManager.Instance.SendToClient(ClientSocket, heartbeatAck);
                    break;
                
                case MessageType.C2S_Revive:
                    Vector3 revivePos = GetSpawnPosition();
                    transform.position = revivePos;
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
                    InputDataConverter.Unpack(data, out uint sequence, out var samples);
                    _inputBuffer.Enqueue(sequence, samples);
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
            float offset = UnityServerManager.Instance.TimeOffset.GetValueOrDefault(playerId, 0f);
            float serverBoneTime = State.clientTime + offset;
            _hitHistory.Add(new HitSnapshot
            {
                ServerTime = serverBoneTime,
                Shapes = _currentShapes
            });
        }

        private void UnpackAnimState(AnimDataFlat dto)   
        {
            int s = dto.PlayerBoolState;
            State.isGround  = (s & (int)PlayerMoveFlags.Ground)  != 0;
            State.isJump    = (s & (int)PlayerMoveFlags.Jump)    != 0;
            State.isSprint  = (s & (int)PlayerMoveFlags.Sprint)  != 0;
            State.isCrouch  = (s & (int)PlayerMoveFlags.Crouch)  != 0;
            State.isSliding = (s & (int)PlayerMoveFlags.Sliding) != 0;

            int w = dto.WeaponBoolState;
            State.isShoot   = (w & (int)WeaponFlags.Shoot)   != 0;
            State.isReload  = (w & (int)WeaponFlags.Reload)   != 0;
            State.isThrow   = (w & (int)WeaponFlags.Throw)    != 0;
            
            if (dto.AimPos != null) State.aimPosition = new Vector3(dto.AimPos.Value.X, dto.AimPos.Value.Y, dto.AimPos.Value.Z);
            
            _combatProvider?.SetWeaponBoolState(dto.WeaponBoolState);
            State.clientTime = dto.ClientTime;
        }


        private void RewindHitCheck(ShootDataFlat shoot)
        {
            if (shoot.Position == null || shoot.Forward == null) return;
            if (shoot.ShooterId == 0) return;

            Vector3 origin = new Vector3(shoot.Position.Value.X, shoot.Position.Value.Y, shoot.Position.Value.Z);
            Vector3 forward = new Vector3(shoot.Forward.Value.X, shoot.Forward.Value.Y, shoot.Forward.Value.Z);
            Ray ray = new Ray(origin, forward);

            // 估算射击时刻的服务端时间（延迟补偿）
            float shooterOffset = UnityServerManager.Instance.TimeOffset.GetValueOrDefault(shoot.ShooterId, 0f);
            float serverShotTime = shoot.ClientTime + shooterOffset;

            float closestDist = float.MaxValue;
            int bestVictimId = -1;
            HitPart bestPart = HitPart.Body;

            foreach (var kv in UnityServerManager.Instance.AllPlayers)
            {
                int victimId = kv.Key;
                if (victimId == shoot.ShooterId) continue;
                var victim = kv.Value;

                // 粗筛：包围球
                if (!CapsuleRayMath.RayBoundingSphere(origin, forward, victim.State.position, 2f))
                    continue;

                // 取最接近射击时刻的快照
                if (!victim._hitHistory.FindClosest(serverShotTime, out var snap)) continue;
                if (snap.Shapes == null) continue;

                // 遍历受害者所有判定框
                foreach (var shape in snap.Shapes)
                {
                    float t = CapsuleRayMath.Intersect(ray, shape);
                    if (t > 0f && t < closestDist && t < 100f)
                    {
                        closestDist = t;
                        bestVictimId = victimId;
                        bestPart = shape.Part;
                    }
                }
            }

            if (bestVictimId == -1) return;

            // 应用伤害
            int damage = bestPart == HitPart.Head
                ? (int)shoot.HeadHit
                : (int)shoot.BaseHit;
            var target = UnityServerManager.Instance.GetPlayer(bestVictimId);

            HurtComponent component = new HurtComponent
            {
                attackerId = playerId,
                Element = shoot.Element,
                ElementMeasure = shoot.ElementMeasure,
                Harm = damage,
                HitType = (int)bestPart,
                Transform = target.transform,
                victimId = bestVictimId
            };

            HurtComponentSystem.ProcessElementReaction(component, target._buffController);
            
            HitDto dto = new HitDto()
            {
                attackerId = shoot.ShooterId,
                victimId = bestVictimId,
                damage = component.Harm,
                element = shoot.Element,
                elementMeasure = shoot.ElementMeasure,
                hitType = (int)bestPart
            };

            UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_DamageConfirm,
                HitDataConverter.ToBinary(dto)));
            component.SkipReaction = true;
            HurtSystem.Instance.EnQueue(component);

            var hpMsg = PlayerDataConverter.PackHp(bestVictimId,
                target.State.curHp, target.State.curShield,
                target.State.maxHp, target.State.maxShield);
            UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_Hp, hpMsg));

            if (target.State.curHp <= 0)
            {
                target.State.curHp = 0;
                target.State.curShield = 0;
                byte[] deathData = BitConverter.GetBytes(bestVictimId);
                UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_PlayerDeath, deathData));
            }
        }
        
        

        /// <summary>
        ///  复活点选择
        /// </summary>
        private Vector3 GetSpawnPosition()
        {
            // todo 后续做安全位置检查
            return Vector3.zero;
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
        public void UpdateHp(float val)
        {
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
                byte[] deathData = BitConverter.GetBytes(playerId);
                UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_PlayerDeath, deathData));
            }
        }

    }
}