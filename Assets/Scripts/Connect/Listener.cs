using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Connect.Converter;
using Connect.DTO;
using Connect.FlatDTO;
using Connect.ShootDTO;
using Firearms;
using Google.FlatBuffers;
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
            Thread listen = new Thread(ListenServer);
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
            if (NetworkMonitor.Instance) // 监听数据包大小
            {
                // 计算全包大小 = 4(总长) + 4(Type长) + Type字符串字节数 + Data体大小
                int typeBytesCount = System.Text.Encoding.UTF8.GetByteCount(msg.type);
                int totalPacketSize = 4 + 4 + typeBytesCount + msg.data.Length;
                
                NetworkMonitor.Instance.LogReceived(msg.type, totalPacketSize);
            }
            
            // 将 byte[] 包装成 ByteBuffer
            ByteBuffer bb = new ByteBuffer(msg.data);
            
            switch (msg.type)
            {
                case "AllocId":
                    // 如果 AllocId 发的是 4 字节整数
                    int newId = BitConverter.ToInt32(msg.data, 0);
                    UserClient.MyId = newId;
                    if (!PlayerPool.Instance.Models.ContainsKey(newId))
                    {
                        PlayerPool.Instance.Models.Add(newId, UserClient.PlayerManager);
                    }
                    break;
                case "PlayerShoot":
                    // 这里的 msg.data 已经是解包后的 byte[]
                    if (NetworkService.Bridge.GetMyId() != -1) 
                    {
                        var tempFlat = ShootDataFlat.GetRootAsShootDataFlat(bb);
        
                        if (tempFlat.ShooterId != UserClient.MyId) 
                        {
                            if (PlayerPool.Instance.Models.TryGetValue(tempFlat.ShooterId, out var m))
                            {
                                
                            }
                        }
                    }
                    break;
                case "AllPlayerInfo":
                    // 解析全量玩家列表
                    var playerListFlat = PlayerList.GetRootAsPlayerList(bb);
                    PlayerPool.Instance.UpdatePlayer(playerListFlat); 
                    break;

                case "DamageConfirm":
                    OnReceiveDamageConfirm(msg);
                    break;

                case "PlayerExit":
                    // 如果发的是 4 字节整数
                    int exitId = BitConverter.ToInt32(msg.data, 0);
                    PlayerPool.Instance.DeletePlayer(exitId);
                    break;
            }
        }
        
        private static void ListenServer(object obj)
        {
            Socket sclisten = (Socket)obj;
            byte[] headerBuff = new byte[4]; // 用来读长度头

            while (true)
            {
                try
                {
                    // 先读 4 字节，确定接下来的包有多大
                    int headLen = sclisten.Receive(headerBuff, 0, 4, SocketFlags.None);
                    if (headLen < 4) break;

                    int bodySize = BitConverter.ToInt32(headerBuff, 0);
                    byte[] bodyBuff = new byte[bodySize];
            
                    // 读取指定长度的内容
                    int received = 0;
                    while (received < bodySize)
                    {
                        received += sclisten.Receive(bodyBuff, received, bodySize - received, SocketFlags.None);
                    }
                    
                    Message msg = UnpackMessage(bodyBuff); 
            
                    lock (lockObj) { toDolist.Enqueue(msg); }
                }
                catch { break; }
            }
        }
        
        public void OnReceiveDamageConfirm(Message msg)
        {
            // 1. 获取 FlatBuffers 根对象
            HitDto dto = new HitDto();
            // 使用转换器解析二进制数据到 dto 对象中
            HitDataConverter.FillData(msg.data, dto);
            
            // 查找 id 判断是否有该玩家
            if (PlayerPool.Instance.Models.TryGetValue(dto.victimId, out var model))
            {
                Debug.Log($"{model.EntityId} 被击中 (数据源: FlatBuffers)");

                HurtComponent component = new HurtComponent()
                {
                    attackerId = dto.attackerId,
                    victimId = dto.victimId,
                    Harm = dto.damage,
                    Element = dto.element,
                    ElementMeasure = dto.elementMeasure,
                    HitType = dto.hitType,
                    Transform = model.transform
                };

                // 进入 HurtSystem
                HurtSystem.Instance.EnQueue(component);
            }
            else Debug.Log($"[Hurt] 未找到该玩家{dto.victimId}");
        }

        private static Message UnpackMessage(byte[] bodyBuff)
        {
            // 读取 Type 字符串的长度 (前4位)
            int typeLen = BitConverter.ToInt32(bodyBuff, 0);

            // 解析 Type 字符串
            string type = System.Text.Encoding.UTF8.GetString(bodyBuff, 4, typeLen);

            // 提取剩余的二进制数据 (FlatBuffers)
            int dataSize = bodyBuff.Length - 4 - typeLen;
            byte[] data = new byte[dataSize];
            Buffer.BlockCopy(bodyBuff, 4 + typeLen, data, 0, dataSize);

            // 返回封装好的 Message 对象
            return new Message(type, data);
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
    }
}