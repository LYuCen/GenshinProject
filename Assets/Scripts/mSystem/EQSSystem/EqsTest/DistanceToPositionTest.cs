using System.Collections.Generic;
using UnityEngine;
using mSystem.EQSSystem.EqsContext;

namespace mSystem.EQSSystem.EqsTest
{
    /// <summary>
    /// 候选点距离目标位置越近分数越高
    /// </summary>
    public class DistanceToPositionTest : IEqsTest
    {
        public float Weight { get; }
        public bool IsOpposite { get; }

        private Vector3 _targetPosition;

        public DistanceToPositionTest(Vector3 targetPosition, float weight = 1f, bool isOpposite = false)
        {
            _targetPosition = targetPosition;
            Weight = weight;
            IsOpposite = isOpposite;
        }

        public void Execute(List<EqsPoint> points, IEqsContext context)
        {
            if (points == null || points.Count == 0) return;

            // 找出点集中最大距离
            float maxLen = 0f;
            foreach (var point in points)
            {
                float dist = Vector3.Distance(point.Position, _targetPosition);
                if (dist > maxLen) maxLen = dist;
            }
            if (maxLen < 0.001f) maxLen = 0.001f; // 防止除以0

            foreach (var point in points)
            {
                float len = Vector3.Distance(point.Position, _targetPosition);
                // 分数计算：距离越近分越高
                float score = Weight * (1f - len / maxLen);

                if (IsOpposite)
                {
                    score = Weight - score; // 取反
                }

                // 分数累加
                point.Score += score;
            }
        }
    }
}