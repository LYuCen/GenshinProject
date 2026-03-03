using System.Collections.Generic;

namespace AI.VisualRequest.VisualRequest
{
    /// <summary>
    /// 混合绘制请求
    /// </summary>
    public class MixVisualRequest : IVisualRequest
    {
        private List<IVisualRequest> _requests = new();
        public bool Enable = true;

        public void AddRequest(IVisualRequest request)
        {
            _requests.Add(request);
        }

        public void Draw(Dictionary<VisualRequestEnum, IVisualizer> visualizers)
        {
            if (Enable)
            {
                foreach (var request in _requests)
                {
                    request.Draw(visualizers);
                }
            }
        }
    }
}