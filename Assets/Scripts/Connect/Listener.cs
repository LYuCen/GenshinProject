using System;
using System.Collections.Generic;
using Connect.Converter;
using Connect.DTO;
using Connect.FlatDTO;
using Connect.Protocol;
using Connect.ShootDTO;
using Firearms;
using Firearms.Bullet;
using Firearms.Muzzle;
using GameFSM.LevelFSM.PlayerFSM;
using Google.FlatBuffers;
using IUtility;
using mModel;
using mSystem.BulletSystem;
using mSystem.HurtSystem;
using UnityEngine;

namespace Connect
{
    public class Listener : MonoBehaviour
    {
        // 单例化，方便模拟器访问
        public static Listener Instance;

        // 统一的消息队列
        private static Queue<Message> toDolist = new Queue<Message>();
        private static readonly object lockObj = new object();

        private void Awake() => Instance = this;
        
        /// <summary>
        /// 向请求队列中添加请求
        /// </summary>
        /// <param name="msg">请求</param>
        public static void EnqueueMessage(Message msg)
        {
            lock (lockObj)
            {
                toDolist.Enqueue(msg);
            }
        }
        
        private void Update()
        {
            while (true)
            {
                Message msg = null;
                lock (lockObj)
                {
                    if (toDolist.Count > 0) msg = toDolist.Dequeue();
                }

                if (msg == null) break;
            
                // 调用统一的处理逻辑
                Dispatch(msg);
            }
        }
        
        /// <summary>
        /// 请求事件处理中心
        /// </summary>
        private void Dispatch(Message msg)
        {
            NetworkService.Instance?.Heartbeat?.OnAnyMessageReceived();
            
            if (NetworkMonitor.Instance) // 监听数据包大小
            {
                // 计算全包大小 = 4(总长) + 2(Type) + Data体大小
                int totalPacketSize = 4 + 2 + msg.data.Length;
                
                NetworkMonitor.Instance.LogReceived(msg.type.ToString(), totalPacketSize);
            }
            
            // 将 byte[] 包装成 ByteBuffer
            ByteBuffer bb = new ByteBuffer(msg.data);
            
            switch (msg.type)
            {
                case MessageType.S2C_AllocId:
                    // 如果 AllocId 发的是 4 字节整数
                    int newId = BitConverter.ToInt32(msg.data, 0);
                    if (NetworkService.Instance && NetworkService.Instance.Client != null)
                    {
                        NetworkService.Instance.Client.MyId = newId;
                        var pm = NetworkService.Instance.playerManager;
                        PlayerPool.Instance.RegisterLocalPlayer(newId, pm);
                        
                    }
                    break;
                case MessageType.S2C_PlayerShoot:
                    // 这里的 msg.data 已经是解包后的 byte[]
                    if (NetworkService.Bridge == null || NetworkService.Bridge.GetMyId() == -1) break;
                    
                    var tempFlat = ShootDataFlat.GetRootAsShootDataFlat(bb);
    
                    if (tempFlat.ShooterId != NetworkService.Bridge.GetMyId()) 
                    {
                        if(tempFlat.Position == null || tempFlat.Forward == null) break;
                        GameObject obj = BulletGameObjectPool.Instance.GetObj();
                        Vector3 pos = new Vector3(tempFlat.Position.Value.X, tempFlat.Position.Value.Y, tempFlat.Position.Value.Z);
                        Vector3 fwd = new Vector3(tempFlat.Forward.Value.X, tempFlat.Forward.Value.Y, tempFlat.Forward.Value.Z);

                        obj.transform.position = pos - fwd * 0.7f;
                        obj.transform.forward = fwd;
                        
                        Bullet bullet = new Bullet()
                        {
                            Transform = obj.transform,
                            shooterId = tempFlat.ShooterId,
                            baseHit = tempFlat.BaseHit,
                            headHit = tempFlat.HeadHit,
                            element = tempFlat.Element,
                            elementMeasure = tempFlat.ElementMeasure,
                            moveNormalize = new Vector3(tempFlat.Forward.Value.X, tempFlat.Forward.Value.Y, tempFlat.Forward.Value.Z),
                            speed = tempFlat.Speed,
                            MoveTime = 3f
                        };
    
                        // 设置粒子和枪焰
                        var ps = bullet.Transform.GetChild(0).GetComponent<ParticleSystem>();
                        var main = ps.main;
                        main.startColor = ElementModel.Instance.colors[bullet.element];
    
                        // 加入子弹系统
                        BulletSystem.Instance.EnQueue(bullet);

                        // 枪口
                        PlayerManager manager = PlayerPool.Instance.Models[tempFlat.ShooterId];
                        if (manager)
                        {
                            GameObject muzzleObj = FireMuzzleObjectPool.Instance.GetObj();
                            muzzleObj.transform.SetParent(manager.CombatModule.RemoteWeapon.muzzle);
                            muzzleObj.gameObject.SetActive(true);
                            muzzleObj.transform.localPosition = Vector3.zero;
                            muzzleObj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            Muzzle muzzle = muzzleObj.GetComponent<Muzzle>();
                            muzzle.UpdateColor(ElementModel.Instance.colors[tempFlat.Element]);

                        }
                    }
                    break;
                case MessageType.S2C_AllPlayerInfo:
                    // 解析全量玩家列表
                    var playerListFlat = PlayerList.GetRootAsPlayerList(bb);
                    PlayerPool.Instance.UpdatePlayer(playerListFlat); 
                    break;

                case MessageType.S2C_DamageConfirm:
                    OnReceiveDamageConfirm(msg);
                    break;

                case MessageType.S2C_PlayerExit:
                    // 如果发的是 4 字节整数
                    int exitId = BitConverter.ToInt32(msg.data, 0);
                    PlayerPool.Instance.DeletePlayer(exitId);
                    break;
                case MessageType.S2C_PlayerSpawn:
                    var spawnFlat = PlayerDataFlat.GetRootAsPlayerDataFlat(bb);
                    PlayerPool.Instance.SpawnRemotePlayer(spawnFlat);
                    break;
                case MessageType.S2C_StateChange:
                    int statePid = BitConverter.ToInt32(msg.data, 0);
                    int stateFlags = BitConverter.ToInt32(msg.data, 4);
                    if (PlayerPool.Instance.Models.TryGetValue(statePid, out var statePm))
                    {
                        var s = statePm.state;
                        s.isGround  = (stateFlags & PlayerStateBoolBit.BIT_GROUND)  != 0;
                        s.isJump    = (stateFlags & PlayerStateBoolBit.BIT_JUMP)    != 0;
                        s.isShoot   = (stateFlags & PlayerStateBoolBit.BIT_SHOOT)   != 0;
                        s.isReload  = (stateFlags & PlayerStateBoolBit.BIT_RELOAD)  != 0;
                        s.isSprint  = (stateFlags & PlayerStateBoolBit.BIT_SPRINT)  != 0;
                        s.isThrow   = (stateFlags & PlayerStateBoolBit.BIT_THROW)   != 0;
                        s.isCrouch  = (stateFlags & PlayerStateBoolBit.BIT_CROUCH)  != 0;
                        s.isSliding = (stateFlags & PlayerStateBoolBit.BIT_SLIDING) != 0;
                        statePm.MoveModule?.SyncFromState(s);
                        statePm.CombatModule?.SyncFromState(s);
                    }
                    break;
                case MessageType.Both_Heartbeat:
                    NetworkService.Instance?.Heartbeat?.OnHeartbeatResponse(msg.data);
                    break;
                
                case MessageType.S2C_Position:
                    var posList = PositionDTO.PositionListFlat.GetRootAsPositionListFlat(bb);
                    PlayerPool.Instance.UpdatePositions(posList);
                    break;
                
                case MessageType.S2C_Hp:
                    PlayerDataConverter.UnPackHp(msg.data, out int id, out float hp, 
                        out float shield, out float maxHp, out float maxShield);
                    PlayerPool.Instance.ApplyHp(id, hp, shield, maxHp, maxShield);
                    break;
                
                case MessageType.S2C_PlayerDeath:
                    int deathId = BitConverter.ToInt32(msg.data, 0);
                    Debug.Log($"玩家 {deathId} 已死亡");
                    break;
            }
        }
        
        public void OnReceiveDamageConfirm(Message msg)
        {
            // 1. 获取 FlatBuffers 根对象
            HitDto dto = new HitDto();
            // 使用转换器解析二进制数据到 dto 对象中
            HitDataConverter.FillData(msg.data, dto);
            
            // 查找 id 判断是否有该玩家
            if (PlayerPool.Instance.Models.TryGetValue(dto.victimId, out var model))
            {
                Debug.Log($"{model.EntityId} 被击中 (数据源: FlatBuffers)");

                HurtComponent component = new HurtComponent()
                {
                    attackerId = dto.attackerId,
                    victimId = dto.victimId,
                    Harm = dto.damage,
                    Element = dto.element,
                    ElementMeasure = dto.elementMeasure,
                    HitType = dto.hitType,
                    Transform = model.transform
                };

                // 进入 HurtSystem
                HurtSystem.Instance.EnQueue(component);
            }
            else Debug.Log($"[Hurt] 未找到该玩家{dto.victimId}");
        }
        
        public static int MessageQueueCount
        {
            get
            {
                lock (lockObj)
                {
                    return toDolist.Count;
                }
            }
        }

        public void BindToClient(UserClient client)
        {
            client.OnMessageReceived += (msg) =>
            {
                lock (lockObj)
                {
                    toDolist.Enqueue(msg);
                }
            };
            client.OnDisconnected += () =>
            {
                NetworkService.Instance.OnClientDisconnected();       
            };
        }
    }
}