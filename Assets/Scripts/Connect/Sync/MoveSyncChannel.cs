using Connect.Converter;
using Connect.Protocol;
using GameFSM.LevelFSM.PlayerFSM;
using UnityEngine;

namespace Connect.Sync
{
    public class MoveSyncChannel : SyncChannel
    {
        private Vector3 _lastPos;
        private Quaternion _lastRot;
        private Vector2 _lastInput;


        public MoveSyncChannel(int id) { 
            Interval = 0.05f; 
            ChannelId = id; 
        } 
        public override bool TrySend(PlayerManager m)
        {
            var s = m.state;
            if (Vector3.Distance(_lastPos, s.position) < 0.001f && 
                Quaternion.Angle(_lastRot, s.rotation) < 0.1f &&
                _lastInput == s.inputMove) return false;

            _lastPos = s.position;
            _lastRot = s.rotation;
            _lastInput = s.inputMove;
            NetworkService.Bridge.SendToServer(
                new Message(MessageType.C2S_Move, PlayerDataConverter.PackMove(s)));
            return true;

        }
    }
}