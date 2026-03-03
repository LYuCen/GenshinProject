using System.Collections.Generic;
using UnityEngine;

namespace AI.VisualRequest.VisualRequest
{
    /// <summary>
    /// 扇形绘制请求
    /// </summary>
    public class SectorVisualRequest : IVisualRequest
    {
        public Vector3 Origin;
        public Vector3 Forward;
        public float Radius;
        public float Angle;
        public Color SectorColor = Color.green;
        public bool Enable = true;

        public void Draw(Dictionary<VisualRequestEnum, IVisualizer> visualizers)
        {
            if (Enable)
            {
                visualizers[VisualRequestEnum.Sector].Draw(this);
            }
        }
    }
}