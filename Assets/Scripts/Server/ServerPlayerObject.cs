using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Connect;
using Connect.Converter;
using Connect.Data;
using Connect.FlatDTO;
using Google.FlatBuffers;
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
            // 二进制下不再使用 StringBuilder
            while (ClientSocket != null && ClientSocket.Connected)
            {
                try
                {
                    // --- 1. 读取全包长度 (4字节) ---
                    byte[] headBuff = new byte[4];
                    if (!ReadExact(ClientSocket, headBuff, 4)) break;
                    int totalSize = BitConverter.ToInt32(headBuff, 0);

                    // --- 2. 读取后续全包内容 ---
                    byte[] bodyBuff = new byte[totalSize];
                    if (!ReadExact(ClientSocket, bodyBuff, totalSize)) break;

                    // --- 3. 拆解二进制包 (对应客户端的 UnpackMessage) ---
                    // 按照 [TypeLen(4)] [TypeStr] [Data] 格式拆分
                    int typeLen = BitConverter.ToInt32(bodyBuff, 0);
                    string type = Encoding.UTF8.GetString(bodyBuff, 4, typeLen);
            
                    int dataOffset = 4 + typeLen;
                    byte[] flatData = new byte[totalSize - dataOffset];
                    Buffer.BlockCopy(bodyBuff, dataOffset, flatData, 0, flatData.Length);

                    // --- 4. 逻辑分发 ---
                    string finalType = type; // 闭包捕获变量
                    UnityServerManager.Instance.EnqueueTask(() => 
                    {
                        HandleBinaryMessage(finalType, flatData);
                    });
                }
                catch (Exception ex)
                {
                    Debug.Log($"玩家 {PlayerId} 接收异常: {ex.Message}");
                    break;
                }
            }
    
            UnityServerManager.Instance.EnqueueTask(() => {
                UnityServerManager.Instance.CloseClient(ClientSocket);
            });
        }

        // 辅助方法：确保读取完整字节
        private bool ReadExact(Socket s, byte[] data, int size)
        {
            int received = 0;
            while (received < size)
            {
                int count = s.Receive(data, received, size - received, SocketFlags.None);
                if (count <= 0) return false;
                received += count;
            }
            return true;
        }
        
        private void HandleBinaryMessage(string type, byte[] data)
        {
            switch (type)
            {
                case "UpdatePosition":
                    PlayerDataConverter.FillData(data, State);
                    transform.position = State.position;
                    transform.rotation = State.rotation;
                    break;

                case "PlayerHitRequest":
                    // 伤害转发：保持二进制透传，性能最高
                    Message confirmMsg = new Message("DamageConfirm", data);
                    UnityServerManager.Instance.Broadcast(confirmMsg);
                    break;

                case "PlayerShoot":
                    // 开火转发：二进制透传
                    Message shootMsg = new Message("PlayerShoot", data);
                    UnityServerManager.Instance.Broadcast(shootMsg);
                    break;
            }
        }
        
        // 获取当前玩家的 Offset (用于全量广播构建列表)
        public Offset<PlayerDataFlat> GetCurrentOffset(FlatBufferBuilder builder)
        {
            // 注意：这里调用你之前在模拟器里写的那个 CreateSinglePlayerOffset 方法
            // 同步物理引擎位置到 State
            State.position = transform.position;
            State.rotation = transform.rotation;
    
            return PlayerDataConverter.CreateSinglePlayerOffset(builder, State);
        }
    }
}