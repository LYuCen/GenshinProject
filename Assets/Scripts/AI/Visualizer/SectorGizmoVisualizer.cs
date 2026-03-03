using AI.VisualRequest;
using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI.Visualizer
{
    /// <summary>
    /// 扇形绘图可视化工具
    /// </summary>
    public class SectorGizmoVisualizer : MonoBehaviour, IVisualizer
    {
        /// <summary>
        /// 绘制扇形
        /// </summary>
        public void Draw(IVisualRequest request)
        {
            if (request is SectorVisualRequest svr)
            {
                float halfAngle = svr.Angle * 0.5f;
                int segments = (int)(60 * (svr.Angle / 360));

                Gizmos.color = svr.SectorColor;

                Vector3 prevPoint = svr.Origin;

                for (int i = 0; i <= segments; i++)
                {
                    float lerp = (float)i / segments;
                    float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, lerp);
                    Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * svr.Forward;
                    Vector3 point = svr.Origin + dir * svr.Radius;

                    if (i > 0)
                        Gizmos.DrawLine(prevPoint, point);

                    if (i == 0 || i == segments)
                        Gizmos.DrawLine(svr.Origin, point);

                    prevPoint = point;
                }
            }
        }
    }
}