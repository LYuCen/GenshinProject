using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Connect;
using Connect.Converter;
using Connect.Data;
using Connect.DTO;
using Newtonsoft.Json;
using UnityEngine;

namespace Server
{
    public class ServerPlayerObject : MonoBehaviour
    {
        public int PlayerId;
        public Socket ClientSocket;
        public Transform[] bones;
        public PlayerStateData State = new PlayerStateData();

        public void Init(int id, Socket s)
        {
            PlayerId = id;
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
            byte[] readbuff = new byte[8192];
            StringBuilder sb = new StringBuilder();

            while (ClientSocket != null && ClientSocket.Connected)
            {
                try
                {
                    int len = ClientSocket.Receive(readbuff);
                    if (len <= 0) break;

                    string str = Encoding.UTF8.GetString(readbuff, 0, len);
                    sb.Append(str);
                    
                    string content = sb.ToString();
                    string[] parts = content.Split('&');

                    // 处理完整消息
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        if (string.IsNullOrWhiteSpace(parts[i])) continue;

                        // 1. 解析外层 Message
                        Message msg = JsonConvert.DeserializeObject<Message>(parts[i]);
                        if (msg == null) continue;
                        
                        if (msg.type == "UpdatePosition")
                        {
                            // 2. 解析内层 DTO
                            PlayerDataDto dto = JsonConvert.DeserializeObject<PlayerDataDto>(msg.info);
                            
                            if (dto != null)
                            {
                                // 3. 【核心修复】调用 Manager 暴露出的任务队列方法
                                UnityServerManager.Instance.EnqueueTask(() => 
                                {
                                    // 这部分代码现在会安全地在主线程运行
                                    OnReceiveUpdate(dto); 
                                });
                            }
                        }
                        else if (msg.type == "PlayerHitRequest")
                        {
                            // 【新增内容】：收到某个客户端发来的攻击请求
                            // 我们直接将其转发给所有人，但把类型改为 "DamageConfirm"
                            // 这样客户端的 Listener 就能识别并扣血了
                            UnityServerManager.Instance.EnqueueTask(() => 
                            {
                                // 构造转发给全场的消息
                                Message confirmMsg = new Message("DamageConfirm", msg.info);
                                UnityServerManager.Instance.Broadcast(confirmMsg);
                
                                Debug.Log($"转发攻击请求: {msg.info}");
                            });
                        }
                        else if (msg.type == "PlayerShoot")
                        {
                            UnityServerManager.Instance.EnqueueTask(() => 
                            {
                                // 构造转发给全场的消息
                                Message confirmMsg = new Message("PlayerShoot", msg.info);
                                UnityServerManager.Instance.Broadcast(confirmMsg);
                            });
                        }
                    }

                    // 保留残余字符串处理断包
                    sb.Clear();
                    sb.Append(parts[parts.Length - 1]);
                }
                catch (Exception ex)
                {
                    Debug.Log($"玩家 {PlayerId} 接收异常: {ex.Message}");
                    break;
                }
            }
            
            // 如果循环跳出，说明断开了，让主线程清理自己
            UnityServerManager.Instance.EnqueueTask(() => {
                UnityServerManager.Instance.CloseClient(ClientSocket);
            });
        }

        private void OnReceiveUpdate(PlayerDataDto dto)
        {
            // 更新服务器上的物理位置，以便射线检测和广播
            transform.position = new Vector3(dto.px, dto.py, dto.pz);
            transform.rotation = new Quaternion(dto.rx, dto.ry, dto.rz, dto.rw);
            
            // 如果你需要同步其他状态（如 isShoot），也要在这里更新
            PlayerDataConverter.FillData(dto, State);
        }
        
        // 获取当前玩家的 DTO 数据用于广播给其他人
        public PlayerDataDto GetCurrentDto()
        {
            // 同步当前物体的物理坐标到 State，然后转为 DTO
            State.position = transform.position;
            State.rotation = transform.rotation;
            return PlayerDataConverter.ToDto(State);
        }
    }
}