using System;
using System.Collections.Generic;
using BaseClass;
using Connect.Converter;
using Connect.DTO;
using Connect.FlatDTO;
using Connect.Protocol;
using Connect.ShootDTO;
using Firearms;
using Firearms.Bullet;
using Connect.ElementBuffDTO;
using Cysharp.Threading.Tasks;
using Firearms.Muzzle;
using GameFSM.LevelFSM.PlayerFSM;
using Google.FlatBuffers;
using IUtility;
using mModel;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.BulletSystem;
using mSystem.HurtSystem;
using mSystem.InputSystem;
using mSystem.ServerTimeSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Connect
{
    public class Listener : AbstractSingleton<Listener>
    {
        // 统一的消息队列
        private static Queue<Message> toDolist = new Queue<Message>();
        private static readonly object lockObj = new object();
        public static event Action<RoomInfoDto> OnRoomInfoReceived;
        public static event Action<List<LobbyPlayerDto>> OnLobbyPlayerReceived;
        public static event Action<ScoreboardDto> OnScoreboardReceived;
        
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
            if (PlayerPool.Instance == null && IsGameSceneMessage(msg.type)) return;
            
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
                    Debug.Log($"[AllocId] 收到服务器分配 ID:{newId}, Client={NetworkService.Instance?.Client?.GetHashCode()}");
                    if (NetworkService.Instance && NetworkService.Instance.Client != null)
                    {
                        NetworkService.Instance.Client.MyId = newId;
                        var pm = NetworkService.Instance.playerManager;
                        if (pm != null) pm.state.playerId = newId;
                    }
                    break;
                case MessageType.S2C_PlayerShoot:
                    // 这里的 msg.data 已经是解包后的 byte[]
                    if (NetworkService.Bridge == null || NetworkService.Bridge.GetMyId() == -1) break;
                    
                    var tempFlat = ShootDataFlat.GetRootAsShootDataFlat(bb);
    
                    if (tempFlat.ShooterId != NetworkService.Bridge.GetMyId()) 
                    {
                        Vector3 pos = new Vector3(tempFlat.PosX / 100f, tempFlat.PosY / 100f, tempFlat.PosZ / 100f);
                        Vector3 fwd = new Vector3(tempFlat.DirX / 127f, tempFlat.DirY / 127f, tempFlat.DirZ / 127f);

                        if (BulletGameObjectPool.Instance)
                        {
                            GameObject obj = BulletGameObjectPool.Instance.GetObj();
                            obj.transform.position = pos - fwd * 0.7f;
                            obj.transform.forward = fwd;
                        
                            Bullet bullet = new Bullet()
                            {
                                Transform = obj.transform,
                                shooterId = tempFlat.ShooterId,
                                baseHit = 0,
                                headHit = 0,
                                element = tempFlat.Element,
                                elementMeasure = 0,
                                moveNormalize = fwd,
                                speed = tempFlat.Speed / 10f,
                                MoveTime = 3f
                            };
    
                            // 设置粒子和枪焰
                            var ps = bullet.Transform.GetChild(0).GetComponent<ParticleSystem>();
                            var main = ps.main;
                            main.startColor = ElementModel.Instance.colors[bullet.element];
    
                            // 加入子弹系统
                            BulletSystem.Instance.EnQueue(bullet);
                        }

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

                    if (NetworkService.Instance != null && NetworkService.Instance.playerManager != null && NetworkService.Bridge.GetMyId() != -1)
                    {
                        PlayerPool.Instance.RegisterLocalPlayer(NetworkService.Bridge.GetMyId(),
                            NetworkService.Instance.playerManager);
                    }
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
                case MessageType.S2C_ElementAttach:
                    OnReceiveElementAttach(bb);
                    break;
                case MessageType.S2C_ElementDetach:
                    OnReceiveElementDetach(bb);
                    break;
                case MessageType.S2C_StatusEffect:
                    OnReceiveStatusEffect(bb);
                    break;
                case MessageType.Both_Heartbeat:
                    NetworkService.Instance?.Heartbeat?.OnHeartbeatResponse(msg.data);
                    break;
                
                case MessageType.S2C_Position:
                    var posList = PositionDTO.PositionListFlat.GetRootAsPositionListFlat(bb);
                    PlayerPool.Instance.UpdatePositions(posList);
                    break;
                
                case MessageType.S2C_Hp:
                    Debug.Log("血条更新");
                    PlayerDataConverter.UnPackHp(msg.data, out int id, out float hp, 
                        out float shield, out float maxHp, out float maxShield);
                    PlayerPool.Instance.ApplyHp(id, hp, shield, maxHp, maxShield);
                    break;
                
                case MessageType.S2C_PlayerDeath:
                    int deathId = BitConverter.ToInt32(msg.data, 0);
                    Debug.Log($"玩家 {deathId} 已死亡");
                    if (PlayerPool.Instance.Models.TryGetValue(deathId, out var deadPm))
                    {
                        PlayerPool.Instance.ApplyHp(deathId, 0, 0, deadPm.state.maxHp, deadPm.state.maxShield);
                    }
                    break;
                
                case MessageType.S2C_Revive:
                    int pid = BitConverter.ToInt32(msg.data, 0);
                    Vector3 posRev = new Vector3(
                        BitConverter.ToSingle(msg.data, 4),    
                        BitConverter.ToSingle(msg.data, 8),    
                        BitConverter.ToSingle(msg.data, 12));  
                    float curHpRev =
                        BitConverter.ToSingle(msg.data, 16);
                    float curShieldRev =
                        BitConverter.ToSingle(msg.data, 20);
                    float maxHpRev =
                        BitConverter.ToSingle(msg.data, 24);
                    float maxShieldRev =
                        BitConverter.ToSingle(msg.data, 28);

                    PlayerPool.Instance.ApplyRevive(pid, posRev, curHpRev, curShieldRev, maxHpRev, maxShieldRev);
                    break;
                
                case MessageType.S2C_StartGame:
                    StartLoadGameScene();
                    break;
                
                case MessageType.S2C_RoomInfo:
                    var roomInfo = RoomInfoConverter.UnpackInfo(msg.data);
                    if (roomInfo != null)
                    {
                        OnRoomInfoReceived?.Invoke(roomInfo);
                        if(OnRoomInfoReceived == null) Debug.Log("OnRoomInfoReceived为空");
                    }
                    else Debug.Log("房间信息获取失败");
                    break;
                
                case MessageType.S2C_LobbyPlayers:
                    OnLobbyPlayerReceived?.Invoke(LobbyPlayerListConverter.UnpackList(msg.data));
                    break;
                
                case MessageType.S2C_Scoreboard:
                    OnScoreboardReceived?.Invoke(ScoreboardConverter.UnPackInfo(msg.data));
                    break;
                
                case MessageType.S2C_GameOver:
                    int endHostId = BitConverter.ToInt32(msg.data, 0);
                    GameEnter.Instance.isGame = false;
                    GameEnter.Instance.returnFromGame = true;
                    GameEnter.Instance.returnIsHost = (NetworkService.Bridge?.GetMyId() == endHostId);
                    
                    BulletGameObjectPool.Instance.Clear();
                    FireMuzzleObjectPool.Instance.Clear();
                    NetworkService.Instance.playerManager = null; 
                    
                    SceneManager.LoadScene("Lobby");
                    break;
            }
        }

        /// <summary>
        /// 判定是否为游戏内请求
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsGameSceneMessage(MessageType type)
        {
            switch (type)
            {
                case MessageType.S2C_PlayerShoot:
                case MessageType.S2C_AllPlayerInfo:
                case MessageType.S2C_DamageConfirm:
                case MessageType.S2C_PlayerExit:
                case MessageType.S2C_PlayerSpawn:
                case MessageType.S2C_StateChange:
                case MessageType.S2C_ElementAttach:
                case MessageType.S2C_ElementDetach:
                case MessageType.S2C_StatusEffect:
                case MessageType.S2C_Position: 
                case MessageType.S2C_Hp:       
                case MessageType.S2C_PlayerDeath:
                case MessageType.S2C_Revive:   
                case MessageType.S2C_GameOver: 
                    return true;
                default:
                    return false;

            }
        }

        private async UniTaskVoid StartLoadGameScene()
        {
            if (SceneManager.GetActiveScene().name == "Level") return;
            try
            {
                await SceneManager.LoadSceneAsync("Level");

                GameEnter.Instance.isGame = true;
                NetworkService.Bridge?.SendToServer(new Message(MessageType.C2S_SceneLoaded, Array.Empty<byte>()));
            }
            catch (Exception e)
            {
                Debug.Log($"开始游戏加载异常： {e.Message}");
            }
        }
        
        public void OnReceiveDamageConfirm(Message msg)
        {
            HitDto dto = new HitDto();
            HitDataConverter.FillData(msg.data, dto);

            if (PlayerPool.Instance.Models.TryGetValue(dto.victimId, out var model))
            {
                HurtSystem.Instance.EnQueue(new HurtComponent
                {
                    attackerId = dto.attackerId,
                    victimId = dto.victimId,
                    Harm = dto.damage,
                    HitType = dto.hitType,
                    Element = dto.element,
                    ElementMeasure = dto.elementMeasure,
                    Transform = model.transform,
                    SkipReaction = true,
                    SkipHp = true
                });
            }
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

        /// <summary>
        /// 元素挂载
        /// </summary>
        private void OnReceiveElementAttach(ByteBuffer bb)
        {
            var flat = ElementAttachFlat.GetRootAsElementAttachFlat(bb);
            if (!PlayerPool.Instance.Models.TryGetValue(flat.TargetId, out var pm)) return;

            var controller = pm.GetComponent<ElementBuffController>();
            if (!controller)
            {
                controller = pm.gameObject.AddComponent<ElementBuffController>();
                controller.id = flat.TargetId;
            }

            controller.isVisualOnly = true;

            int idx = flat.Element;
            float now = Time.time;
            float rate = ElementData.GetDecayRate(flat.Level);
            float attachTime = rate > 0 ? now - (flat.Capacity - flat.Val) / rate : now;
            controller.Elements[idx] = new ElementData
            {
                Level = flat.Level,
                Capacity = flat.Capacity,
                AttachTime = attachTime,
                Enable = true
            };
            // 调度客户端元素衰减事件
            TimeSystem.Instance.Add(new ElementDecayEvent(controller, idx, now));
        }

        /// <summary>
        /// 元素移除
        /// </summary>
        private void OnReceiveElementDetach(ByteBuffer bb)
        {
            var flat = ElementDetachFlat.GetRootAsElementDetachFlat(bb);
            if (!PlayerPool.Instance.Models.TryGetValue(flat.TargetId, out var pm)) return;

            var controller = pm.GetComponent<ElementBuffController>();
            if (!controller) return;

            int idx = flat.Element;
            controller.Elements[idx] = default;
        }

        private void OnReceiveStatusEffect(ByteBuffer bb)
        {
            var flat = StatusEffectFlat.GetRootAsStatusEffectFlat(bb);
            if (!PlayerPool.Instance.Models.TryGetValue(flat.TargetId, out var pm)) return;

            var controller = pm.GetComponent<ElementBuffController>();
            if (!controller)
            {
                controller = pm.gameObject.AddComponent<ElementBuffController>();
                controller.id = flat.TargetId;
            }

            if (flat.IsAdd)
            {
                IStatusEffect effect = flat.EffectType switch
                {
                    0 => new BurnEffect(controller.TimeProvider(), controller),
                    1 => new ElectrifyEffect(controller.TimeProvider(), controller),
                    2 => new SharpenEffect(controller.TimeProvider(), controller),
                    3 => new FreezeEffect(controller.TimeProvider(), controller),
                    _ => null
                };
                if (effect != null) controller.AddEffect(effect);
            }
            else
            {
                Type removeType = flat.EffectType switch
                {
                    0 => typeof(BurnEffect),
                    1 => typeof(ElectrifyEffect),
                    2 => typeof(SharpenEffect),
                    3 => typeof(FreezeEffect),
                    _ => null
                };
                if (removeType != null) controller.RemoveEffectByType(removeType);
            }
        }
    }
}