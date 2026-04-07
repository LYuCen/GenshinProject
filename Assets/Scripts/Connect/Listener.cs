using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Connect.DTO;
using IUtility;
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
        
        public static void StartListen(Socket sc)
        {
            Thread listen = new Thread(new ParameterizedThreadStart(ListenServer));
            listen.IsBackground = true;
            listen.Start(sc);
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
            switch (msg.type)
            {
                case "AllocId":
                    int newId = int.Parse(msg.info);
                    UserClient.MyId = newId;
                    // 如果你要在这里注册本地玩家，确保先判断是否存在
                    if (!PlayerPool.Instance.Models.ContainsKey(newId))
                    {
                        PlayerPool.Instance.Models.Add(newId, UserClient.PlayerManager);
                    }
                    break;
                case "AllPlayerInfo":
                    List<PlayerDataDto> dtos = JsonConvert.DeserializeObject<List<PlayerDataDto>>(msg.info);
                    PlayerPool.Instance.UpdatePlayer(dtos);
                    break;
                case "PlayerShoot": // 只有远程玩家发来的开火才在这里执行
                    ShootDto shootDto = JsonConvert.DeserializeObject<ShootDto>(msg.info);
                    Debug.Log($"监听到{shootDto.shooterId}射击");
                    if (shootDto.shooterId != UserClient.MyId) {
                        // 找到对应的枪执行远程射击逻辑
                        if (PlayerPool.Instance.Models.TryGetValue(shootDto.shooterId, out var m))
                        {
                            m.firearmController.PressPrimaryAction(true, shootDto);
                        }
                    }
                    break;
                case "DamageConfirm":
                    OnReceiveDamageConfirm(msg);
                    break;
                case "PlayerExit":
                    PlayerPool.Instance.DeletePlayer(int.Parse(msg.info));
                    break;
            }
        }
        
        private static void ListenServer(object obj)
        {
            Socket sclisten = (Socket)obj;
            byte[] readbuff = new byte[8192]; // 增大缓冲区防止长 JSON 截断
            while (true)
            {
                try
                {
                    int len = sclisten.Receive(readbuff);
                    if (len <= 0) break;

                    string str = System.Text.Encoding.UTF8.GetString(readbuff, 0, len);
                    foreach (string s in str.Split('&'))
                    {
                        if (string.IsNullOrWhiteSpace(s)) continue;
                        try 
                        {
                            Message msg = JsonConvert.DeserializeObject<Message>(s);
                            // 监听网络
                            if (NetworkMonitor.Instance)
                            {
                                int bytes = System.Text.Encoding.UTF8.GetBytes(s).Length;
                                NetworkMonitor.Instance.LogReceived(msg.type, bytes);
                            }
                            lock (lockObj) { toDolist.Enqueue(msg); }
                        }
                        catch { /* 忽略损坏的包 */ }
                    }
                }
                catch { break; }
            }
        }
        
        public void OnReceiveDamageConfirm(Message msg)
        {
            // 1. 解析服务器发来的“伤害确认”
            HitDto dto = JsonConvert.DeserializeObject<HitDto>(msg.info);
            
            Debug.Log("收到攻击指令");
            
            // 查找 id 判断是否有该玩家
            if (PlayerPool.Instance.Models.TryGetValue(dto.victimId, out var model))
            {
                Debug.Log($"{model.EntityId}被击中");
                Transform target = model.transform;
                HurtComponent component = new HurtComponent()
                {
                    attackerId = dto.attackerId,
                    victimId = dto.victimId,
                    Element = dto.element,
                    ElementMeasure = dto.elementMeasure, // 默认元素量
                    Harm = dto.damage,
                    HitType = dto.hitType,
                    Transform = target
                };

                // 3. 进入 HurtSystem 进行逻辑判定（元素反应、弹出数字、减血）
                HurtSystem.Instance.EnQueue(component);
            }
        }

        
    }
}