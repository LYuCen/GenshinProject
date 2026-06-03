using System;
using mEnum;
using UnityEngine;

namespace Connect.HitRewind
{
    /// <summary>
    /// 拖入 Collider 即可，运行时读取参数自动构建 HitShape
    /// </summary>
    [Serializable]
    public struct HitColliderEntry
    {
        public Collider collider;
        public HitPart  hitPart;
    }

    public static class HitShapeExtractor
    {
        public static void Extract(HitColliderEntry[] entries, HitShape[] output)
        {
            for (int i = 0; i < entries.Length && i < output.Length; i++)
            {
                var entry = entries[i];
                if (entry.collider == null) continue;

                Transform t = entry.collider.transform;

                if (entry.collider is CapsuleCollider cc)
                {
                    FillCapsule(t, cc, entry.hitPart, ref output[i]);
                }
                else if (entry.collider is BoxCollider bc)
                {
                    FillBox(t, bc, entry.hitPart, ref output[i]);
                }
                else if (entry.collider is SphereCollider sc)
                {
                    FillSphere(t, sc, entry.hitPart, ref output[i]);
                }
            }
        }

        static void FillCapsule(Transform t, CapsuleCollider cc, HitPart part, ref HitShape shape)
        {
            float scale = t.lossyScale.y; // 粗略，假设均匀缩放
            float halfH = Mathf.Max(0, cc.height * 0.5f - cc.radius) * scale;
            float radius = cc.radius * Mathf.Max(t.lossyScale.x, t.lossyScale.z);

            Vector3 worldCenter = t.TransformPoint(cc.center);
            Vector3 axis = GetCapsuleAxis(t, cc.direction);

            shape.Type   = HitShapeType.Capsule;
            shape.Part   = part;
            shape.Bottom = worldCenter - axis * halfH;
            shape.Top    = worldCenter + axis * halfH;
            shape.Radius = radius;
        }

        static void FillBox(Transform t, BoxCollider bc, HitPart part, ref HitShape shape)
        {
            Vector3 halfExtents;
            halfExtents.x = bc.size.x * 0.5f * t.lossyScale.x;
            halfExtents.y = bc.size.y * 0.5f * t.lossyScale.y;
            halfExtents.z = bc.size.z * 0.5f * t.lossyScale.z;

            shape.Type     = HitShapeType.Box;
            shape.Part     = part;
            shape.Bottom   = t.TransformPoint(bc.center); // 盒子世界中心
            shape.Top      = halfExtents;                  // 世界半长宽高
            shape.Rotation = t.rotation;
        }

        static void FillSphere(Transform t, SphereCollider sc, HitPart part, ref HitShape shape)
        {
            float radius = sc.radius * Mathf.Max(t.lossyScale.x,
                Mathf.Max(t.lossyScale.y, t.lossyScale.z));
            Vector3 worldCenter = t.TransformPoint(sc.center);

            shape.Type   = HitShapeType.Capsule; // 球体用胶囊退化：Bottom==Top
            shape.Part   = part;
            shape.Bottom = worldCenter;
            shape.Top    = worldCenter;
            shape.Radius = radius;
        }

        static Vector3 GetCapsuleAxis(Transform t, int direction)
        {
            return direction switch
            {
                0 => t.right,    // X-Axis
                1 => t.up,       // Y-Axis
                2 => t.forward,  // Z-Axis
                _ => t.up
            };
        }
    }
}
