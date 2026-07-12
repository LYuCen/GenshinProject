using System.Collections.Generic;
using System.Linq;
using GameFSM.LevelFSM.PlayerFSM;
using UnityEngine;

namespace Connect.Sync
{
    public class PlayerSyncManager
    {
        private List<SyncChannel> _channels = new();

        public void Register(SyncChannel channel)
        {
            _channels.Add(channel);
        }

        public void UnRegister(SyncChannel channel)
        {
            foreach (var sc in _channels.ToList())
            {
                if (sc == channel) _channels.Remove(channel);
            }
        }

        public void OnFixedUpdate(PlayerManager m)
        {
            float dt = Time.fixedDeltaTime;
            foreach (var sc in _channels)
            {
                sc.Timer += dt;
                if (sc.Timer >= sc.Interval)
                {
                    sc.Timer -= sc.Interval;
                    sc.TrySend(m);
                }
            }
        }
    }
}