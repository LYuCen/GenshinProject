using System;
using System.Net.Sockets;
using System.Threading;
using Connect;
using Connect.Converter;
using Connect.Data;
using Connect.DTO;
using Connect.FlatDTO;
using Connect.Protocol;
using Connect.ShootDTO;
using Google.FlatBuffers;
using mEnum;
using mInterface;
using mSystem.StatusSystem;
using Player;
using UnityEngine;

namespace Server
{
    public class ServerPlayerObject : MonoBehaviour, IEntityIdentity
    {
        public int EntityId => playerId;
        public EntityStatusUI EntityStatusUI => null;
        public int playerId;
        public Socket ClientSocket;
        public Transform[] bones;
        public PlayerStateData State = new PlayerStateData();

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
            
            // 启动接收线程
            Thread t = new Thread(ReceiveLoop);
            t.IsBackground = true;
            t.Start();
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
                    ServerAuthoritativeRaycast(shootFlat);
                    break;
                
                case MessageType.Both_Heartbeat:
                    UnityServerManager.Instance.UpdateHeartbeat(playerId);
                    float serverTime = Time.time;
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

        private void ServerAuthoritativeRaycast(ShootDataFlat shoot)
        {
            if (shoot.Position == null || shoot.Forward == null) return;
            
            Vector3 origin = new Vector3(shoot.Position.Value.X, shoot.Position.Value.Y, shoot.Position.Value.Z);
            Vector3 forward = new Vector3(shoot.Forward.Value.X, shoot.Forward.Value.Y, shoot.Forward.Value.Z);

            float maxRange = 100f;

            int hitboxMask = 1 << LayerMask.NameToLayer("Hitbox");

            if (Physics.Raycast(origin, forward, out RaycastHit hit, maxRange, hitboxMask))
            {
                var hitBox = hit.collider.GetComponent<HitBox>();
                if (hitBox == null || hitBox.Owner == null) return;
                
                int victimId = hitBox.Owner.EntityId;
                if (victimId == shoot.ShooterId) return;

                int damage = (hitBox.part == HitPart.Head) ? (int)shoot.HeadHit : (int)shoot.BaseHit;

                var victim = UnityServerManager.Instance.GetPlayer(victimId);
                victim.State.curShield -= damage;
                if (victim.State.curShield < 0)
                {
                    victim.State.curHp += victim.State.curShield;
                    victim.State.curShield = 0;
                }

                HitDto dto = new HitDto()
                {
                    attackerId = shoot.ShooterId,
                    victimId = victimId,
                    damage = damage,
                    element = shoot.Element,
                    elementMeasure = shoot.ElementMeasure,
                    hitType = (int)hitBox.part
                };

                UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_DamageConfirm,
                    HitDataConverter.ToBinary(dto)));

                var hpMsg = PlayerDataConverter.PackHp(victimId, victim.State.curHp, victim.State.curShield, victim.State.maxHp, victim.State.maxShield);        
                UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_Hp, hpMsg));

                
                if (victim.State.curHp <= 0)
                {
                    victim.State.curHp = 0;
                    victim.State.curShield = 0;

                    // 通知所有客户端该玩家死亡
                    byte[] deathData = BitConverter.GetBytes(victimId);
                    UnityServerManager.Instance.Broadcast(new Message(MessageType.S2C_PlayerDeath, deathData));
                }
            }
        }
    }
}