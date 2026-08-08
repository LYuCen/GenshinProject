using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using Connect.ConnectPlayer;
using Connect.FlatDTO;
using Connect.PositionDTO;
using GameFSM.LevelFSM.PlayerFSM;
using Player.ManagerLogic;
using UnityEngine;

namespace Connect
{
    public class PlayerPool : MonoBehaviour
    {
        // Key 是玩家 ID，Value 是 PlayerManager（包含数据和逻辑壳）
        private Dictionary<int, PlayerManager> _models = new Dictionary<int, PlayerManager>();
        public Dictionary<int, PlayerManager> Models => _models;
        public static PlayerPool Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdatePlayer(PlayerList list)
        {
            if (NetworkService.Bridge.GetMyId() == -1) return;
            
            // FlatBuffers 的数组遍历方式：使用 Length 和索引器
            for (int i = 0; i < list.PlayersLength; i++)
            {
                // 获取单个玩家数据，注意加上 .Value，因为返回的是可空类型
                var dto = list.Players(i).Value;
                int pid = dto.PlayerId;
                Debug.Log($"[UpdatePlayer] pid={pid},inPool={_models.ContainsKey(pid)},myId={NetworkService.Bridge.GetMyId()}");
                if (_models.ContainsKey(pid))
                {
                    PlayerManager pm = _models[pid];
                    
                    pm.HpModule.SyncStatus(dto); 

                    if (pid == NetworkService.Bridge.GetMyId()) continue;

                    if (pm.GetLogic() is RemotePlayerLogic remoteLogic)
                    {
                        remoteLogic.SyncFromServer(dto);
                        if (pm.TryGetComponent<RemotePlayer>(out var rp))
                        {
                            var pos = dto.Position.HasValue
                                ? new Vector3(dto.Position.Value.X, dto.Position.Value.Y, dto.Position.Value.Z)
                                : pm.transform.position;

                            var rot = dto.Rotation.HasValue
                                ? new Quaternion(dto.Rotation.Value.X, dto.Rotation.Value.Y, dto.Rotation.Value.Z, dto.Rotation.Value.W)
                                : pm.transform.rotation;

                            rp.OnServerUpdate(pos, rot, Vector2.zero, pm.aimIK.solver.target.position);
                        }
                        else
                        {
                            Debug.LogWarning($"玩家 {pid} 缺少 RemotePlayer 组件！");
                        }

                    }
                }
                else
                {
                    if (pid == NetworkService.Bridge.GetMyId()) continue;

                    // 创建远程玩家
                    CreateRemotePlayer(dto);
                }
            }
        }

        public void SpawnRemotePlayer(PlayerDataFlat dto)
        {
            CreateRemotePlayer(dto);
        }

        // 提取创建逻辑
        private void CreateRemotePlayer(PlayerDataFlat dto)
        {
            IResource ir = ResourceManager.instance.Load(
                "Assets/AssetBundle/Model/Player/RemoteLumine.prefab", false);

            GameObject prefab = ir.GetAsset<GameObject>();
            GameObject instance = Instantiate(prefab);

            // 访问坐标方式：dto.Position.Value.X
            if (dto.Position != null)
            {
                var pos = dto.Position.Value;
                instance.transform.position = new Vector3(pos.X, pos.Y, pos.Z);
            }

            instance.name = "RemotePlayer_" + dto.PlayerId;
            
            PlayerManager pm = instance.GetComponent<PlayerManager>();
            pm.state.playerId = dto.PlayerId;
            pm.state.curHp = dto.CurHp;
            pm.state.maxHp = dto.MaxHp;
            pm.state.curShield = dto.CurShield; 
            pm.state.maxShield = dto.MaxShield;
            pm.Init(false);

            if (pm.GetLogic() is RemotePlayerLogic remotePlayerLogic)
            {
                remotePlayerLogic.SyncFromServer(dto);
            }

            // 初始状态同步到模块，确保首帧 FSM 读取正确的状态
            pm.MoveModule?.SyncFromState(pm.state);
            pm.CombatModule?.SyncFromState(pm.state);

            _models.Add(dto.PlayerId, pm);
            Debug.Log($"检测到新玩家加入，生成实体: {dto.PlayerId}");
        }

        /// <summary>
        /// 本地玩家初始化时调用这个，把自己存进去
        /// </summary>
        public void RegisterLocalPlayer(int id, PlayerManager pm)
        {
            if (!_models.ContainsKey(id))
            {
                _models.Add(id, pm);
                Debug.Log($"本地玩家已注册到 Pool, ID: {id}");
            }
        }

        public void DeletePlayer(int id)
        {
            if (_models.ContainsKey(id))
            { 
                // 彻底销毁或放回池子
                Destroy(_models[id].gameObject);
                _models.Remove(id);
                Debug.Log($"玩家 {id} 已下线，清理资源。"); 
            }
        }

        public void UpdatePositions(PositionListFlat list)
        {
            if (NetworkService.Bridge == null || NetworkService.Bridge.GetMyId() == -1) return;

            for (int i = 0; i < list.PlayersLength; i++)
            {
                var dto = list.Players(i).Value;
                int pid = dto.PlayerId;
                
                if(!_models.TryGetValue(pid, out var pm)) continue;
                if(pid == NetworkService.Bridge.GetMyId()) continue;

                if (pm.TryGetComponent<RemotePlayer>(out var rp))
                {
                    var pos = dto.Position.HasValue
                        ? new Vector3(dto.Position.Value.X, dto.Position.Value.Y, dto.Position.Value.Z)
                        : pm.transform.position;
                    var rot = dto.Rotation.HasValue 
                        ? new Quaternion(dto.Rotation.Value.X, dto.Rotation.Value.Y, dto.Rotation.Value.Z, dto.Rotation.Value.W)
                        : pm.transform.rotation;
                    var inputMove = new Vector2(dto.InputX, dto.InputY);
                    var aimPos = dto.Aim.HasValue
                        ? new Vector3(dto.Aim.Value.X, dto.Aim.Value.Y, dto.Aim.Value.Z)
                        : pm.aimIK.solver.target.position;
                    
                    rp.OnServerUpdate(pos, rot, inputMove, aimPos);

                }
            }
        }

        public void ApplyHp(int playerId, float curHp, float curShield, float maxHp, float maxShield)
        {
            if (_models.TryGetValue(playerId, out var pm))
            {
                pm.state.curHp = curHp;
                pm.state.maxHp = maxHp;
                pm.state.curShield = curShield;
                pm.state.maxShield = maxShield;
                pm.entityStatusUI?.SyncValues(curHp, maxHp, curShield, maxShield);
            }
        }
        
        public void ClearPool()
        {
            foreach (var pm in _models.Values)
            {
                if (pm && pm.gameObject && pm.EntityId != NetworkService.Bridge.GetMyId())
                {
                    // 单机转联机时清除不删除自己
                    if (pm.EntityId == 1000) continue;
                    Destroy(pm.gameObject);
                }
            }
            _models.Clear();
            Debug.Log("PlayerPool 已清空，准备联机。");
        }
        
        /// <summary>
        /// 复活逻辑
        /// </summary>
        public void ApplyRevive(int playerId, Vector3 pos, float curHp, float curShield, float maxHp, float maxShield)
        {
            if (!_models.TryGetValue(playerId, out var pm)) return;

            pm.transform.position = pos;
            pm.state.position = pos;
            pm.state.curHp = curHp;
            pm.state.maxHp = maxHp;
            pm.state.curShield = curShield;
            pm.state.maxShield = maxShield;
            pm.entityStatusUI?.SyncValues(curHp, maxHp, curShield, maxShield);
        }
        
        public void ClearBots()
        {  
            var toRemove = new List<int>();
            foreach (var kv in _models)
            {
                if (kv.Key == 1000) continue; // 保留主控玩家
                if (kv.Value && kv.Value.gameObject)
                    Destroy(kv.Value.gameObject);
                toRemove.Add(kv.Key);
            }
            foreach (var id in toRemove)
                _models.Remove(id);
        }

    }
}