using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;
using UnityEngine;

namespace mSystem.EQSSystem.EqsGenerator
{
    /// <summary>
    /// 固定半径 + 固定角度圆形生成器（均匀分布在圆周）
    /// </summary>
    public class CircleEqsGenerator : IEqsGenerator
    {
        public Vector3 Center;
        public float Radius = 5f;           // 圆半径
        public int PointCount = 8;          // 点数量
        public float HeightOffset = 0f;     // 高度偏移
        public float RayOriginHeight = 10f; // 投射高度
        public LayerMask GroundMask = LayerMask.GetMask("Plane");        // 地面层

        public List<EqsPoint> Generate(IEqsContext context)
        {
            List<EqsPoint> result = new List<EqsPoint>(PointCount);

            if (!context?.Querier) return result;

            if (Center == Vector3.zero) Center = context.Querier.position;

            float angleStep = 360f / PointCount;

            for (int i = 0; i < PointCount; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * Radius,
                    0,
                    Mathf.Sin(angle) * Radius
                );

                Vector3 worldPos = Center + offset;

                // 从高处往下投射
                Vector3 rayOrigin = worldPos + Vector3.up * RayOriginHeight;

                if (Physics.Raycast(
                        rayOrigin,
                        Vector3.down,
                        out RaycastHit hit,
                        RayOriginHeight * 2f,
                        GroundMask))
                {
                    result.Add(new EqsPoint
                    {
                        Position = hit.point + Vector3.up * HeightOffset
                    });
                }
            }

            return result;
        }
    }
}