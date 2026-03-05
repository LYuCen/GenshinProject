using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;
using UnityEngine;

namespace mSystem.EQSSystem.EqsTest
{
    /// <summary>
    /// 视线检测
    /// </summary>
    public class SightTargetTest : IEqsTest
    {
        
        public float Weight { get; }
        public bool IsOpposite { get; }
        
        /// <summary>
        /// 可检测的障碍层
        /// </summary>
        private LayerMask _obstacleMask;

        public SightTargetTest(float weight = 1f, bool isOpposite = false, LayerMask? obstacleMask = null)
        {
            Weight = weight;
            IsOpposite = isOpposite;
            _obstacleMask = obstacleMask ?? LayerMask.GetMask("Default"); // 默认挡住所有Default层
        }

        public void Execute(List<EqsPoint> points, IEqsContext context)
        {
            if (!context?.Querier || !context.Target) return;

            Vector3 targetPos = context.Target.position;

            foreach (var point in points)
            {
                // 从候选点到玩家连线
                Vector3 dir = targetPos - point.Position;
                float distance = dir.magnitude;

                bool blocked = Physics.Raycast(point.Position, dir.normalized, distance, _obstacleMask);

                // 判断是否有效
                bool valid = IsOpposite ? blocked : !blocked;

                point.IsValid &= valid;
                point.Score += valid ? Weight : 0f;
            }
        }
    }
}