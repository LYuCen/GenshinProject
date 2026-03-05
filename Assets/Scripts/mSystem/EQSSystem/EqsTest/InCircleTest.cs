using System.Collections.Generic;
using UnityEngine;
using mSystem.EQSSystem.EqsContext;

namespace mSystem.EQSSystem.EqsTest
{
    /// <summary>
    /// 圆形范围测试：候选点在指定圆内为有效
    /// </summary>
    public class InCircleTest : IEqsTest
    {
        public float Weight { get; }
        public bool IsOpposite { get; }

        /// <summary>
        /// 圆心位置
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// 圆半径
        /// </summary>
        public float Radius;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="center">圆心</param>
        /// <param name="radius">半径</param>
        /// <param name="weight">权重</param>
        /// <param name="isOpposite">是否取反</param>
        public InCircleTest(Vector3 center, float radius, float weight = 1f, bool isOpposite = false)
        {
            Center = center;
            Radius = radius;
            Weight = weight;
            IsOpposite = isOpposite;
        }

        public void Execute(List<EqsPoint> points, IEqsContext context)
        {
            foreach (var point in points)
            {
                // 计算点到圆心距离
                float dist = Vector3.Distance(point.Position, Center);

                // 判断是否在圆内
                bool inside = dist <= Radius;

                // 根据 IsOpposite 取反
                bool valid = IsOpposite ? !inside : inside;

                point.IsValid &= valid;
                point.Score += valid ? Weight : 0f;
            }
        }
    }
}