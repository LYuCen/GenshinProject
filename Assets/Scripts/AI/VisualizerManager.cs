using System.Collections.Generic;
using AI.Visualizer;
using AI.VisualRequest;
using BaseClass;
using Unity.VisualScripting;

namespace AI
{
    public class VisualizerManager : AbstractSingleton<VisualizerManager>
    {
        private List<IVisualRequest> _requests = new List<IVisualRequest>();

        private Dictionary<VisualRequestEnum, IVisualizer> _dictionary = new Dictionary<VisualRequestEnum, IVisualizer>();

        protected override void Awake()
        {
            base.Awake();
            _dictionary.Add(VisualRequestEnum.Point, this.AddComponent<PointVisualizer>());
            _dictionary.Add(VisualRequestEnum.Line, this.AddComponent<LineVisualizer>());
            _dictionary.Add(VisualRequestEnum.Sector, this.AddComponent<SectorGizmoVisualizer>());
            _dictionary.Add(VisualRequestEnum.Circle, this.AddComponent<CircleVisualizer>());
        }
        
        
        
        public void Register(IVisualRequest request)
        {
            if (!_requests.Contains(request))
            {
                _requests.Add(request);
            }
        }

        public void Unregister(IVisualRequest request)
        {
            if (_requests.Contains(request)) _requests.Remove(request);
        }

        private void OnDrawGizmos()
        {
            foreach (var req in _requests)
            {
                req.Draw(_dictionary);
            }
        }

        public void ClearAll()
        {
            _requests.Clear();
        }
    }
}