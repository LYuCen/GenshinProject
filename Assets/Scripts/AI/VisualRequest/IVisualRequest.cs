using System.Collections.Generic;

namespace AI.VisualRequest
{
    public interface IVisualRequest
    {
        void Draw(Dictionary<VisualRequestEnum, IVisualizer> visualizers);
    }
}