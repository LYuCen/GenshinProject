using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;
using UnityEngine;

namespace mSystem.EQSSystem.EqsGenerator
{
    public class GridEqsGenerator : IEqsGenerator
    {
        public float Length = 10f;      // 网格长度
        public float Width = 10f;       // 网格宽度
        public float HeightOffset = 0f; // 高度偏移量
        public float SpaceBetween = 1f; // 点间距
        public float RayOriginHeight = 10f; // 投射高度
        public LayerMask GroundMask = LayerMask.GetMask("Plane"); // 地面层

        public List<EqsPoint> Generate(IEqsContext context)
        {
            int halfX = Mathf.FloorToInt(Length * 0.5f / SpaceBetween);
            int halfZ = Mathf.FloorToInt(Width * 0.5f / SpaceBetween);

            int totalCount = (halfX * 2 + 1) * (halfZ * 2 + 1);
            List<EqsPoint> result = new List<EqsPoint>(totalCount);

            Vector3 center = context.Querier.position;
            Quaternion rotation = context.Querier.rotation;

            for (int x = -halfX; x <= halfX; x++)
            {
                for (int z = -halfZ; z <= halfZ; z++)
                {
                    Vector3 offset = new Vector3(
                        x * SpaceBetween,
                        0,
                        z * SpaceBetween
                    );

                    Vector3 rotatedOffset = rotation * offset;

                    Vector3 worldPos = center + rotatedOffset;

                    // 从高处往下投射寻找合法地面
                    Vector3 rayOrigin = worldPos + Vector3.up * RayOriginHeight;

                    if (Physics.Raycast(
                            rayOrigin,
                            Vector3.down,
                            out RaycastHit hit,
                            20f,
                            GroundMask))
                    {
                        result.Add(new EqsPoint
                        {
                            Position = hit.point + Vector3.up * HeightOffset
                        });
                    }
                }
            }

            return result;
        }
    }
}