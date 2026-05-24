using mWeapon.ComponentData;
using UnityEngine;

namespace mWeapon.Component
{
    public class TransformComponent
    {
        public Vector3 Pos;
        public Vector3 Rot;

        public TransformComponent(TransformComponentData data)
        {
            Pos = data.pos;
            Rot = data.rot;
        }
    }
}