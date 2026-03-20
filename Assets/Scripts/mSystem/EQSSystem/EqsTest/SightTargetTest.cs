using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;
using UnityEngine;

namespace mSystem.EQSSystem.EqsTest
{
    /// <summary>
    /// 视线检测玩家是否可见
    /// </summary>
    public class SightTargetTest : IEqsTest
    {
        
        public float Weight { get; }

        public bool IsOpposite
        {
            get => _isOpposite;
            set => _isOpposite = value;
        }

        /// <summary>
        /// 点功能（true表示如果验证不通过该点不可用）
        /// </summary>
        private bool _pointAble;

        /// <summary>
        /// 可检测的障碍层
        /// </summary>
        private LayerMask _obstacleMask;

        private bool _isOpposite;

        public SightTargetTest(float weight = 1f, bool isOpposite = false, LayerMask? obstacleMask = null, bool pointAble = false)
        {
            Weight = weight;
            _isOpposite = isOpposite;
            _obstacleMask = obstacleMask ?? LayerMask.GetMask("Default"); // 默认挡住所有Default层
            _pointAble = pointAble;
        }

        public void Execute(List<EqsPoint> points, IEqsContext context)
        {
            if (!context?.Querier || !context.Target) return;

            Vector3 targetPos = context.Target.position;
            Vector3 heightOffset = new Vector3(0, 1.4f, 0);

            foreach (var point in points)
            {
                Vector3 start = point.Position + heightOffset;
                Vector3 end = targetPos + heightOffset;

                Vector3 dir = end - start;
                float distance = dir.magnitude;

                bool blocked = Physics.Raycast(start, dir.normalized, distance, _obstacleMask);

                bool valid = _isOpposite ? blocked : !blocked;

                if(_pointAble) point.IsValid &= valid;
                point.Score += valid ? Weight : 0f;
            }
        }

        public void OppositeBool()
        {
            _isOpposite = !IsOpposite;
        }
    }
}