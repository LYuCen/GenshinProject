using AI.VisualRequest;
using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI.Visualizer
{
    /// <summary>
    /// 点可视化工具
    /// </summary>
    public class PointVisualizer : MonoBehaviour, IVisualizer
    {
        /// <summary>
        /// 绘制点和线
        /// </summary>
        public void Draw(IVisualRequest request)
        {
            if (request is PointVisualRequest pvr)
            {
                // 绘制终点圆球
                Gizmos.color = pvr.PointColor;
                Gizmos.DrawSphere(pvr.PointPos, 0.2f);
            }
        }
    }
}