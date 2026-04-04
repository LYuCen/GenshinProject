using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Connect.DTO;
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
                    UserClient.MyId = int.Parse(msg.info);
                    break;
                case "AllPlayerInfo":
                    List<PlayerDataDto> dtos = JsonConvert.DeserializeObject<List<PlayerDataDto>>(msg.info);
                    PlayerPool.Instance.UpdatePlayer(dtos);
                    break;
                case "PlayerShoot": // 只有远程玩家发来的开火才在这里执行
                    ShootDto shootDto = JsonConvert.DeserializeObject<ShootDto>(msg.info);
                    if (shootDto.shooterId != UserClient.MyId) {
                        // 找到对应的枪执行远程射击逻辑
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
            byte[] readbuff = new byte[2048]; // 增大缓冲区防止长 JSON 截断
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
            
            // 查找 id 判断是否有该玩家
            if (PlayerPool.Instance.Models.TryGetValue(dto.victimId, out var model))
            {
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