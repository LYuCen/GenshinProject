using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using Connect.DTO;
using GameFSM.FSM.PlayerFSM;
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

        public void UpdatePlayer(List<PlayerDataDto> dtos)
        {
            // 如果还没领到 ID，先不处理同步逻辑，避免分不清谁是谁
            if (UserClient.MyId == -1) return;

            foreach (PlayerDataDto dto in dtos)
            {
                if (_models.ContainsKey(dto.playerId))
                {
                    PlayerManager pm = _models[dto.playerId];
                    pm.SyncStatus(dto); // 同步血量、护盾等数值

                    // 如果是本地玩家，更新完数值就够了，位移由本地控制
                    if (dto.playerId == UserClient.MyId) continue;

                    // 如果是远程玩家，同步位移和动画
                    if (pm.GetLogic() is RemotePlayerLogic remoteLogic)
                    {
                        remoteLogic.SyncFromServer(dto);
                    }
                }
                else
                {
                    if (dto.playerId == UserClient.MyId)
                    {
                        continue; 
                    }

                    // 3. 创建远程玩家
                    CreateRemotePlayer(dto);
                }
            }
        }

        // 提取创建逻辑
        private void CreateRemotePlayer(PlayerDataDto dto)
        {
            IResource ir = ResourceManager.instance.Load(
                "Assets/AssetBundle/Model/Weapon/Player/RemoteLumine.prefab", false);

            GameObject prefab = ir.GetAsset<GameObject>();
            GameObject instance = Instantiate(prefab);

            instance.transform.position = new Vector3(dto.px, dto.py, dto.pz);
            instance.name = "RemotePlayer_" + dto.playerId;
            
            PlayerManager pm = instance.GetComponent<PlayerManager>();
            pm.state.curHp = dto.curHp;
            pm.state.maxHp = dto.maxHp;
            pm.Init(false); // 远程模式
            
            _models.Add(dto.playerId, pm);
            Debug.Log($"检测到新玩家加入，生成实体: {dto.playerId}");
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