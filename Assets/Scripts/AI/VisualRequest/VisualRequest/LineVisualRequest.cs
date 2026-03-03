using System.Collections.Generic;
using UnityEngine;

namespace AI.VisualRequest.VisualRequest
{
    /// <summary>
    /// 直线绘制请求
    /// </summary>
    public class LineVisualRequest : IVisualRequest
    {
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Color LineColor;
        public bool Enable = true;

        public void Draw(Dictionary<VisualRequestEnum, IVisualizer> visualizers)
        {
            if(Enable) visualizers[VisualRequestEnum.Line].Draw(this);
        }
    }
}