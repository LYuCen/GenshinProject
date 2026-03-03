using AI.VisualRequest;
using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI.Visualizer
{
    /// <summary>
    /// 圆形绘制工具
    /// </summary>
    public class CircleVisualizer : MonoBehaviour, IVisualizer
    {
        public void Draw(IVisualRequest request)
        {
            if (request is CircleVisualRequest cvr)
            {
                if (!cvr.Enable)
                    return;

                Gizmos.color = cvr.Color;

                int segments = Mathf.Max(12, cvr.Segments); 
                float angleStep = 360f / segments;

                Vector3 prevPoint = cvr.Center + 
                                    new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * cvr.Radius;

                for (int i = 1; i <= segments; i++)
                {
                    float angle = angleStep * i * Mathf.Deg2Rad;

                    Vector3 newPoint = cvr.Center +
                                       new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * cvr.Radius;

                    Gizmos.DrawLine(prevPoint, newPoint);
                    prevPoint = newPoint;
                }
            }
        }
    }
}