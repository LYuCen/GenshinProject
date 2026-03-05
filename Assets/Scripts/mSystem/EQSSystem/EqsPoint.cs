using UnityEngine;

namespace mSystem.EQSSystem
{
    public class EqsPoint
    {
        public Vector3 Position;
        public float Score;
        public bool IsValid = true;

        public void Reset()
        {
            Score = 0;
            IsValid = true;
        }
    }
}