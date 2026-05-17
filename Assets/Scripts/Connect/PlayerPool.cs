using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using Connect.FlatDTO;
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
            if (UserClient.MyId == -1) return;

            // FlatBuffers 的数组遍历方式：使用 Length 和索引器
            for (int i = 0; i < list.PlayersLength; i++)
            {
                // 获取单个玩家数据，注意加上 .Value，因为返回的是可空类型
                var dto = list.Players(i).Value;
                int pid = dto.PlayerId;

                if (_models.ContainsKey(pid))
                {
                    PlayerManager pm = _models[pid];
                    
                    pm.HpModule.SyncStatus(dto); 

                    if (pid == UserClient.MyId) continue;

                    if (pm.GetLogic() is RemotePlayerLogic remoteLogic)
                    {
                        // --- 重要：你需要修改 remoteLogic.SyncFromServer 让其支持 PlayerDataFlat ---
                        remoteLogic.SyncFromServer(dto);
                    }
                }
                else
                {
                    if (pid == UserClient.MyId) continue;

                    // 创建远程玩家
                    CreateRemotePlayer(dto);
                }
            }
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
            pm.Init(false); 
            
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
        
        public void ClearPool()
        {
            foreach (var pm in _models.Values)
            {
                if (pm && pm.gameObject && pm.EntityId != UserClient.MyId)
                {
                    Destroy(pm.gameObject);
                }
            }
            _models.Clear();
            Debug.Log("PlayerPool 已清空，准备联机。");
        }
    }
}