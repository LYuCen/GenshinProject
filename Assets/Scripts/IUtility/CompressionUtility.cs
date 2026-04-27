using UnityEngine;

namespace IUtility
{
    public class CompressionUtility
    {
        private const float HalfRange = 1.0f;
        private const float Precision = 65535f;

        // 压缩：Float -> ushort
        public static ushort Compress(float value)
        {
            // 映射 [-1.0, 1.0] 到 [0.0, 1.0]
            float normalized = (value + 1f) * 0.5f;
            return (ushort)(Mathf.Clamp01(normalized) * Precision);
        }

        // 解压：ushort -> Float
        public static float Decompress(ushort value)
        {
            // 映射 [0, 65535] 到 [-1.0, 1.0]
            return (value / Precision) * 2.0f - 1.0f;
        }

        public static Vector3 GetPos(AnimationCurve x, AnimationCurve y, AnimationCurve z, float t)
        {
            return new Vector3(x.Evaluate(t), y.Evaluate(t), z.Evaluate(t));
        }

        public static Quaternion GetRot(AnimationCurve x, AnimationCurve y, AnimationCurve z, float t)
        {
            return Quaternion.Euler(x.Evaluate(t), y.Evaluate(t), z.Evaluate(t));
        }

    }
}