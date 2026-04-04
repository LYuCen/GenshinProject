using System.Collections.Generic;
using UnityEngine;

namespace AI.VisualRequest.VisualRequest
{
    /// <summary>
    /// 点绘制请求
    /// </summary>
    public class PointVisualRequest : IVisualRequest
    {
        public Vector3 PointPos;
        public float Radius = 0.2f;
        public Color PointColor = Color.yellow; // 圆球颜色
        public string Text = null; // 新增字段，用于显示数字或文字
        public bool Enable = true;

        public void Draw(Dictionary<VisualRequestEnum, IVisualizer> visualizers)
        {
            if(Enable) visualizers[VisualRequestEnum.Point].Draw(this);
        }
        
    }
}