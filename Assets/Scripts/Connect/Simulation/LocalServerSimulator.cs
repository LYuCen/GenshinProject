using System;
using System.Collections.Generic;
using Connect.Converter;
using Connect.Data;
using Connect.FlatDTO;
using GameFSM.FSM.PlayerFSM;
using Google.FlatBuffers;
using UnityEngine;

namespace Connect.Simulation
{
    public class LocalServerSimulator : INetworkBridge
    {
        private PlayerManager _playerManager; // 单机玩家的 Manager
        private int _myId = 1000; // 单机模式给个固定ID
        private int _nextBotId = 5000; // Bot 从 5000 开始编号，避免冲突
        
        // 全局玩家列表
        private List<PlayerStateData> _fakeBots = new List<PlayerStateData>();
        private FlatBufferBuilder _builder = new FlatBufferBuilder(1024);
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
                maxHp = 200, curHp = 200,
                maxShield = 100, curShield = 100,
                bRots = Array.Empty<Transform>()
            };

            // 加入模拟服务器的列表
            _fakeBots.Add(data);

            // 2. 使用 FlatBuffers 打包全量玩家列表
            byte[] listData = PackAllPlayers(_fakeBots);

            // 3. 广播给客户端
            ReceiveAtClient(new Message("AllPlayerInfo", listData));
        
            Debug.Log($"<color=green>模拟器：已生成 Bot {_nextBotId - 1}，总人数：{_fakeBots.Count}</color>");
        }
        
        // 将玩家列表打包成二进制
        private byte[] PackAllPlayers(List<PlayerStateData> playerList)
        {
            // 1. 初始化 Builder
            _builder.Clear(); // 建议复用类成员变量，减少 GC

            // 2. 创建所有玩家的 Offset 列表
            Offset<PlayerDataFlat>[] playerOffsets = new Offset<PlayerDataFlat>[playerList.Count];
            for (int i = 0; i < playerList.Count; i++)
            {
                playerOffsets[i] = PlayerDataConverter.CreateSinglePlayerOffset(_builder, playerList[i]);
            }

            // 3. 创建 Vector (玩家列表数组)
            var playersVector = PlayerList.CreatePlayersVector(_builder, playerOffsets);

            // 4. 创建 Root Table (PlayerList)
            PlayerList.StartPlayerList(_builder);
            PlayerList.AddPlayers(_builder, playersVector);
            var root = PlayerList.EndPlayerList(_builder);

            // 5. 结束构建
            _builder.Finish(root.Value);
            return _builder.SizedByteArray();
        }

        private void ReceiveAtClient(Message msg)
        {
            // 直接调用客户端解析消息的方法
            Listener.EnqueueMessage(msg);
        }
    }
}