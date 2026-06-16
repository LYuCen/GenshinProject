using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using BaseClass;
using Connect;
using Connect.Converter;
using Connect.Data;
using Connect.DTO;
using Connect.FlatDTO;
using Connect.HitRewind;
using Connect.Protocol;
using Connect.ShootDTO;
using Google.FlatBuffers;
using mEnum;
using mInterface;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem;
using mSystem.HurtSystem;
using mSystem.StatusSystem;
using UnityEngine;

namespace Server
{
    public class ServerPlayerObject : MonoBehaviour, IEntityIdentity, IManager
    {
        public int EntityId => playerId;
        public EntityStatusUI EntityStatusUI => null;
        public int playerId;
        public Socket ClientSocket;
        public Transform[] bones;
        public PlayerStateData State = new PlayerStateData();
        public HitColliderEntry[] hitColliders = new HitColliderEntry[11];
        private HitHistoryBuffer _hitHistory;
        private HitShape[] _currentShapes;

        private IElementBuffSystem _buffSystem;
        private ElementBuffController _buffController;

        public void Init(int id, Socket s)
        {
            playerId = id;
            ClientSocket = s;
            State.playerId = id;
            State.bRots = bones; 
            
            State.maxHp = 200f;
            State.curHp = 200f;
            State.maxShield = 100f;
            State.curShield = 100f;
            _hitHistory = new HitHistoryBuffer(8);
            _currentShapes = new HitShape[11];
            
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
                
                case MessageType.C2S_Move:
                    PlayerDataConverter.UnpackMove(data, State);
                    transform.position = State.position;
                    transform.rotation = State.rotation;
                    
                    break;
                
                
                case MessageType.C2S_Bones:
                    PlayerDataConverter.UnpackBone(data, State);
                    CacheSnapshotFromColliders();
                    break;
                
                case MessageType.C2S_Revive:
                    transform.position = Vector3.zero;
                    State.position = Vector3.zero;
                    State.curHp = State.maxHp;
                    State.curShield = State.maxShield;

                    var hpData = PlayerDataConverter.PackHp(playerId, State.curHp, State.curShield, State.maxHp, State.maxShield);     
                    UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_Hp, hpData));
                    break;
                
                case MessageType.C2S_StateChange:
                    int flags = BitConverter.ToInt32(data, 0);
                    State.isGround  = (flags & PlayerStateBoolBit.BIT_GROUND)  != 0;
                    State.isJump    = (flags & PlayerStateBoolBit.BIT_JUMP)    != 0;
                    State.isShoot   = (flags & PlayerStateBoolBit.BIT_SHOOT)   != 0;
                    State.isReload  = (flags & PlayerStateBoolBit.BIT_RELOAD)  != 0;
                    State.isSprint  = (flags & PlayerStateBoolBit.BIT_SPRINT)  != 0;
                    State.isThrow   = (flags & PlayerStateBoolBit.BIT_THROW)   != 0;
                    State.isCrouch  = (flags & PlayerStateBoolBit.BIT_CROUCH)  != 0;
                    State.isSliding = (flags & PlayerStateBoolBit.BIT_SLIDING) != 0;

                    // 广播给其他玩家: [playerId(4) + flags(4)]
                    byte[] flagBroadcast = new byte[8];
                    Buffer.BlockCopy(BitConverter.GetBytes(playerId), 0, flagBroadcast, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(flags), 0, flagBroadcast, 4, 4);
                    UnityServerManager.Instance.BroadcastExcept(new Message(MessageType.S2C_StateChange, flagBroadcast), playerId);
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