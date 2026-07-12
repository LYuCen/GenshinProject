using GameFSM.LevelFSM.PlayerFSM;

namespace Connect.Sync
{
    /// <summary>
    /// 频道基类
    /// </summary>
    public abstract class SyncChannel
    {
        public float Interval;  // 发送间隔
        public float Timer;     // 累积时间
        public int ChannelId;   // 频道标识

        public abstract bool TrySend(PlayerManager m);
    }
}