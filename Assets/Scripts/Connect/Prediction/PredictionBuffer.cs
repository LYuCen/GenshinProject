using System.Text.RegularExpressions;
using Connect.Converter;
using GameFSM.LevelFSM.LocomotionFSM;
using UnityEngine;

namespace Connect.Prediction
{
    public class PredictionBuffer
    {
        private TickRecord[] _buffer;
        private int _capacity;

        public PredictionBuffer(int capacity = 256)
        {
            _capacity = capacity;
            _buffer = new TickRecord[capacity];
        }

        public void Save(uint tick, Vector3 position, Locomotion loco, InputDataConverter.InputSample input)
        {
            int idx = (int)(tick % (uint)_capacity);

            _buffer[idx].Tick = tick;
            _buffer[idx].HasData = true;
            _buffer[idx].Position = position;
            _buffer[idx].Input = input;
            _buffer[idx].State = Capture(loco);
        }

        private LocomotionSnapshot Capture(Locomotion loco)
        {
            return new LocomotionSnapshot
            {
                LevelSpeed = loco.LevelSpeed,
                VerticalSpeed = loco.VerticalSpeed,
                IsSprint = loco.IsSprint,
                IsCrouch = loco.IsCrouch,
                IsJump = loco.IsJump,
                IsGround = loco.IsGround,
                MoveVec = loco.MoveVec,
                InputMoveVec = loco.InputMoveVec,
                Yaw = loco.Yaw
            };
        }

        public void Restore(Locomotion loco, LocomotionSnapshot snap)
        {
            loco.LevelSpeed = snap.LevelSpeed;
            loco.VerticalSpeed = snap.VerticalSpeed;

            loco.IsSprint = snap.IsSprint;
            loco.IsCrouch = snap.IsCrouch;
            loco.IsJump = snap.IsJump;
            loco.IsGround = snap.IsGround;
            loco.MoveVec = snap.MoveVec;
            loco.InputMoveVec = snap.InputMoveVec;
            loco.Yaw = snap.Yaw;
        }

        public bool TryGet(uint tick, out TickRecord record)
        {
            int idx = (int)(tick % (uint)_capacity);

            record = _buffer[idx];
            return record.HasData && record.Tick == tick;
        }

        public void UpdatePosition(uint tick, Vector3 position)
        {
            int idx = (int)(tick % (uint)_capacity);
            if (_buffer[idx].HasData && _buffer[idx].Tick == tick)
            {
                _buffer[idx].Position = position;
            }
        }
    }
}