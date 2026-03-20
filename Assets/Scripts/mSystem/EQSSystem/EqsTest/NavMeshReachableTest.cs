using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using mSystem.EQSSystem.EqsContext;

namespace mSystem.EQSSystem.EqsTest
{
    /// <summary>
    /// NavMesh 判断候选点是否可达
    /// </summary>
    public class NavMeshReachableTest : IEqsTest
    {
        public float Weight { get; }
        public bool IsOpposite
        {
            get => _isOpposite;
            set => _isOpposite = value;
        }
        private int _areaMask;
        private bool _isOpposite;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="weight">分值权重</param>
        /// <param name="isOpposite">取反</param>
        /// <param name="areaMask">NavMesh 可达区域</param>
        public NavMeshReachableTest(float weight = 1f, bool isOpposite = false, int areaMask = NavMesh.AllAreas)
        {
            Weight = weight;
            _isOpposite = isOpposite;
            _areaMask = areaMask;
        }

        public void Execute(List<EqsPoint> points, IEqsContext context)
        {
            if (context?.Querier == null) return;

            Vector3 startPos = context.Querier.position;

            foreach (var point in points)
            {
                // 判断点是否可达
                NavMeshPath path = new NavMeshPath();
                bool reachable = NavMesh.CalculatePath(startPos, point.Position, _areaMask, path) 
                                 && path.status == NavMeshPathStatus.PathComplete;

                // 根据 IsOpposite 取反逻辑
                bool valid = IsOpposite ? !reachable : reachable;

                point.IsValid &= valid;

                // 分值就用权重
                point.Score += valid ? Weight : 0f;
            }
        }
        
        public void OppositeBool()
        {
            _isOpposite = !IsOpposite;
        }
    }
}