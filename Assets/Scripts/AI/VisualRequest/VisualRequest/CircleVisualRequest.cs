using System.Collections.Generic;
using UnityEngine;

namespace AI.VisualRequest.VisualRequest
{
    /// <summary>
    /// 圆形绘制请求
    /// </summary>
    public class CircleVisualRequest : IVisualRequest
    {
        public Vector3 Center;
        public float Radius;
        public Color Color = Color.green;
        public bool Enable = true;
        public int Segments = 36;

        public void Draw(Dictionary<VisualRequestEnum, IVisualizer> visualizers)
        {
            visualizers[VisualRequestEnum.Circle].Draw(this);
        }
    }
}