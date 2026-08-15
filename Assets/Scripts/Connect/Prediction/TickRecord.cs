using Connect.Converter;
using UnityEngine;

namespace Connect.Prediction
{
    public struct TickRecord
    {
        public uint Tick;
        public bool HasData;
        public Vector3 Position;
        public LocomotionSnapshot State;
        public InputDataConverter.InputSample Input;
    }
}