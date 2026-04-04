using System.Collections.Generic;
using Connect.Converter;
using Connect.Data;
using Connect.DTO;
using GameFSM.FSM.PlayerFSM;
using Newtonsoft.Json;
using UnityEngine;

namespace Connect.Simulation
{
    public class LocalServerSimulator : INetworkBridge
    {
        private PlayerManager _playerManager; // 单机玩家的 Manager
        private int _myId = 1000; // 单机模式给个固定ID
        private int _nextBotId = 5000; // Bot 从 5000 开始编号，避免冲突
        
        // 全局玩家列表
        private List<PlayerDataDto> _fakeWorldPlayers = new List<PlayerDataDto>();
        public int GetMyId() => _myId;

        // 模拟服务器的消息处理器
        public void SendToServer(Message msg)
        {
            // 模拟服务器处理逻辑
            HandleServerLogic(msg);
        }

        public void InitPlayerManager(PlayerManager manager)
        {
            Debug.Log("单机角色初始化成功！");
            _playerManager = manager;
            
            if (!PlayerPool.Instance.Models.ContainsKey(_myId))
            {
                PlayerPool.Instance.Models.Add(_myId, _playerManager);
            }
        }

        private void HandleServerLogic(Message msg)
        {
            switch (msg.type)
            {
                case "PlayerShoot":
                    // 模拟服务器广播：直接把消息传给客户端的消息监听器
                    // 此处也可配置延迟
                    ReceiveAtClient(msg);
                    break;
                    
                case "UpdatePosition":
                    // 单机模式下，位置更新通常不需要传回自己
                    break;
                
                case "PlayerHitRequest":
                    Message confim = new Message("DamageConfirm", msg.info);
                    ReceiveAtClient(confim);
                    break;
                case "AddBot":
                    CreateFakeBot();
                    break;
            }
        }
        
        private void CreateFakeBot()
        {
            // 初始化一个在玩家脚下的 bot
            PlayerStateData data = new PlayerStateData()
            {
                playerId = _nextBotId++,
                isGround = true,
                position = _playerManager.transform.position,
                rotation = _playerManager.transform.rotation,
                maxHp = 200,
                curHp = 200,
                maxShield = 100,
                curShield = 100
            };

            // 加入模拟服务器的列表
            _fakeWorldPlayers.Add(PlayerDataConverter.ToDto(data));

            // 模拟服务器广播 AllPlayerInfo 给客户端
            // 注意：真实服务器会发所有人，模拟器这里只要发包含所有 Bot 的列表即可
            string json = JsonConvert.SerializeObject(_fakeWorldPlayers);
            ReceiveAtClient(new Message("AllPlayerInfo", json));
            
            Debug.Log($"<color=green>模拟器：已生成 Bot {_nextBotId - 1}</color>");
        }

        private void ReceiveAtClient(Message msg)
        {
            // 直接调用客户端解析消息的方法
            Listener.EnqueueMessage(msg);
        }
    }
}