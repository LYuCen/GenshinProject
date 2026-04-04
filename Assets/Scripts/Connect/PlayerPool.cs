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
            foreach (PlayerDataDto dto in dtos)
            {
                if (_models.ContainsKey(dto.playerId))
                {
                    // 2. 更新已存在的玩家
                    PlayerManager pm = _models[dto.playerId];
                    pm.SyncStatus(dto); // 同步状态数值
                    
                    // 过滤自己
                    if (dto.playerId == UserClient.MyId || UserClient.MyId == -1) continue;
                    
                    // 获取该 Manager 挂载的远程逻辑
                    // 假设你在 PlayerManager 中提供了一个访问 Logic 的方法，或者直接在这里取
                    if (pm.GetLogic() is RemotePlayerLogic remoteLogic)
                    {
                        // 使用转换器将 DTO 填入逻辑层的同步方法中
                        remoteLogic.SyncFromServer(dto);
                    }
                }
                else
                {
                    // 3. 创建新玩家
                    IResource ir =
                        ResourceManager.instance.Load(
                            "Assets/AssetBundle/Model/Weapon/Player/RemoteLumine.prefab",
                            false);

                    GameObject prefab = ir.GetAsset<GameObject>();
                    GameObject instance = Instantiate(prefab);

                    instance.transform.position = new Vector3(dto.px, dto.py, dto.pz);
                    instance.name = "RemotePlayer_" + dto.playerId;
                    
                    PlayerManager pm = instance.GetComponent<PlayerManager>();
                    
                    pm.state.curHp = dto.curHp;
                    pm.state.maxHp = dto.maxHp;
                    pm.state.curShield = dto.curShield;
                    pm.state.maxShield = dto.maxShield;
                    
                    // 初始化为远程模式（这会创建 RemotePlayerLogic）
                    pm.Init(false); 
                    
                    _models.Add(dto.playerId, pm);

                    // 初始同步一次
                    if (pm.GetLogic() is RemotePlayerLogic remoteLogic)
                    {
                        remoteLogic.SyncFromServer(dto);
                    }
                }
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
    }
}