using Connect.Converter;
using Connect.Protocol;
using GameFSM.LevelFSM.PlayerFSM;
using mModel;
using UnityEngine;

namespace Connect.Sync
{
    public class AnimSyncChannel : SyncChannel
    {
        private BagModel _bagModel;

        private int _lastMoveQuery;
        private int _lastCombatQuery;
        private byte _lastRuntimeState;
        private Vector3 _lastRayEnd;

        public AnimSyncChannel(int id, BagModel model) { 
            Interval = 0.1f; 
            ChannelId = id;
            _bagModel = model;
        } 
        public override bool TrySend(PlayerManager m)
        {
            int moveState = m.MoveModule.GetQuery().State;
            int combatState = m.CombatModule.GetQuery().State;
            byte stance = (byte)_bagModel.Weapon.InternalRuntime.Stance; 
            Vector3 rayEnd = m.cameraController.rayEnd;
            
            if (moveState == _lastMoveQuery &&
                combatState == _lastCombatQuery &&
                stance == _lastRuntimeState &&
                Vector3.Distance(_lastRayEnd, rayEnd) < 0.05f)
                return false;
            
            _lastMoveQuery = moveState;
            _lastCombatQuery = combatState;
            _lastRuntimeState = stance;
            _lastRayEnd = rayEnd;
            
            byte[] data = PlayerDataConverter.PackAnimationState(m.EntityId, m.MoveModule.GetQuery().State,
                m.CombatModule.GetQuery().State, (byte)_bagModel.Weapon.InternalRuntime.Stance,
                m.cameraController.rayEnd, Time.time, m.state.clientTime);
            return true;
        }
    }
}