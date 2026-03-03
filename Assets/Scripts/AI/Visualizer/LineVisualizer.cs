using AI.VisualRequest;
using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI.Visualizer
{
    /// <summary>
    /// 直线绘制工具
    /// </summary>
    public class LineVisualizer : MonoBehaviour, IVisualizer
    {
        public void Draw(IVisualRequest request)
        {
            if (request is LineVisualRequest lvr)
            {
                // 绘制起点到终点的线
                Gizmos.color = lvr.LineColor;
                Gizmos.DrawLine(lvr.StartPos, lvr.EndPos);
            }
        }
    }
}