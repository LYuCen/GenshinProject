using AI.VisualRequest;
using AI.VisualRequest.VisualRequest;
using UnityEditor;
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
                
                // 绘制文字，编译环境下执行
                #if UNITY_EDITOR
                if (pvr.Text != null)
                {
                    Handles.color = Color.white;
                    // pvr.Text可以是数字、索引、权重等
                    Handles.Label(pvr.PointPos + Vector3.up * 0.5f, pvr.Text);
                }
                #endif
            }
        }
    }
}