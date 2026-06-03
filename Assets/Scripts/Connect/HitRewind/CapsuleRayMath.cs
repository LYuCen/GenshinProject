using UnityEngine;

namespace Connect.HitRewind
{
    public static class CapsuleRayMath
    {
        /// <summary>
        /// 射线与 HitShape 求交，返回命中距离，-1 表示未命中
        /// </summary>
        public static float Intersect(Ray ray, HitShape shape)
        {
            return shape.Type switch
            {
                HitShapeType.Capsule => RayCapsule(ray, shape.Bottom, shape.Top, shape.Radius),
                HitShapeType.Box => RayOBB(ray, shape.Bottom, shape.Top, shape.Rotation), 
                _ => -1f
            };
        }

        // ==================== Capsule ====================

        /// <summary>
        /// 射线与胶囊体求交
        /// </summary>
        public static float RayCapsule(Ray ray, Vector3 bottom, Vector3 top, float radius)
        {
            Vector3 ba = top - bottom;
            float baLen = ba.magnitude;

            // 轴向长度为零 → 退化为球体
            if (baLen < 0.0001f) return RaySphere(ray.origin, ray.direction, bottom, radius);

            Vector3 d = ba / baLen; // 轴向单位向量
            Vector3 v = ray.origin - bottom;

            // 投影到垂直于轴向的平面上解无穷圆柱
            float dDotD  = Vector3.Dot(ray.direction, ray.direction); // 通常为 1
            float dDotR  = Vector3.Dot(d, ray.direction);
            float dDotV  = Vector3.Dot(d, v);
            float vDotR  = Vector3.Dot(v, ray.direction);
            float vDotV  = Vector3.Dot(v, v);

            float a = dDotD - dDotR * dDotR;   // |D_⊥|²
            float b = vDotR - dDotV * dDotR;    // v_⊥ · D_⊥
            float c = vDotV - dDotV * dDotV - radius * radius;

            float bestT = float.MaxValue;

            if (a > 0.0001f)
            {
                float disc = b * b - a * c;
                if (disc >= 0f)
                {
                    float sqrtD = Mathf.Sqrt(disc);
                    CheckCylinderRoot((-b - sqrtD) / a);
                    CheckCylinderRoot((-b + sqrtD) / a);
                }
            }

            // 端点球体
            bestT = MinPositive(bestT, RaySphere(ray.origin, ray.direction, bottom, radius));
            bestT = MinPositive(bestT, RaySphere(ray.origin, ray.direction, top, radius));

            return bestT < float.MaxValue ? bestT : -1f;

            // 检查圆柱交点的投影是否在 [0, baLen] 范围内
            void CheckCylinderRoot(float t)
            {
                if (t <= 0f) return;
                float proj = dDotV + t * dDotR;
                if (proj >= 0f && proj <= baLen)
                    bestT = Mathf.Min(bestT, t);
            }
        }

        // ==================== OBB ====================

        /// <summary>
        /// 射线与有向包围盒求交（slab 法）
        /// </summary>
        /// <param name="center">盒子中心（世界坐标由 HitShape.Bottom 传入）</param>
        /// <param name="halfExtents">半长宽高（世界坐标由 HitShape.Top 传入）</param>
        /// <param name="rotation">朝向（世界坐标由 HitShape.Rotation 传入）</param>
        public static float RayOBB(Ray ray, Vector3 center, Vector3 halfExtents, Quaternion rotation)
        {
            // 变换到 OBB 局部坐标系
            Quaternion inv = Quaternion.Inverse(rotation);
            Vector3 lo = inv * (ray.origin - center);
            Vector3 ld = inv * ray.direction;

            float tMin = 0f;
            float tMax = float.MaxValue;

            for (int i = 0; i < 3; i++)
            {
                float origin = lo[i];
                float dir    = ld[i];
                float h      = halfExtents[i];

                if (Mathf.Abs(dir) < 0.0001f)
                {
                    // 射线平行于该轴，起点超出则未命中
                    if (origin < -h || origin > h) return -1f;
                    continue;
                }

                float t1 = (-h - origin) / dir;
                float t2 = ( h - origin) / dir;
                if (t1 > t2) // 交换
                {
                    (t1, t2) = (t2, t1);
                }

                tMin = Mathf.Max(tMin, t1);
                tMax = Mathf.Min(tMax, t2);
                if (tMin > tMax) return -1f;
            }

            return tMin;
        }

        // ==================== 粗筛 ====================

        /// <summary>
        /// 射线与包围球相交判定，粗筛用。dir 视为单位向量
        /// </summary>
        public static bool RayBoundingSphere(Vector3 origin, Vector3 dir,
            Vector3 center, float radius)
        {
            Vector3 oc = origin - center;
            float b = Vector3.Dot(oc, dir);
            float c = Vector3.Dot(oc, oc) - radius * radius;
            return b > 0f || b * b - c >= 0f;
        }

        // ==================== 内部工具 ====================

        static float RaySphere(Vector3 origin, Vector3 dir, Vector3 center, float radius)
        {
            Vector3 oc = origin - center;
            float a = Vector3.Dot(dir, dir);
            float b = Vector3.Dot(oc, dir);
            float c = Vector3.Dot(oc, oc) - radius * radius;
            float disc = b * b - a * c;
            if (disc < 0f) return float.MaxValue;
            float t = (-b - Mathf.Sqrt(disc)) / a;
            return t > 0f ? t : float.MaxValue;
        }

        static float MinPositive(float a, float b)
        {
            if (a <= 0f) return b;
            if (b <= 0f) return a;
            return a < b ? a : b;
        }
    }
}
