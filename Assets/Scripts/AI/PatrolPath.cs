using System.Collections.Generic;
using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI
{
    public class PatrolPath : MonoBehaviour
    {
        public bool loop = true;
        public List<Transform> points;

        private MixVisualRequest _mixVisualRequest;
        private List<PointVisualRequest> _points = new List<PointVisualRequest>();
        private List<LineVisualRequest> _lines = new List<LineVisualRequest>();

        private void Awake()
        {
            points = new List<Transform>();

            foreach (Transform child in transform)
            {
                points.Add(child);
            }

            _mixVisualRequest = new MixVisualRequest();
            
            for (int i = 0; i < points.Count; i++)
            {
                PointVisualRequest pvr = new PointVisualRequest();
                pvr.PointColor = Color.gray;
                pvr.PointPos = points[i].position;
                _mixVisualRequest.AddRequest(pvr);
                _points.Add(pvr);
                if (i > 0)
                {
                    LineVisualRequest lvr = new LineVisualRequest();
                    lvr.StartPos = points[i - 1].position;
                    lvr.EndPos = points[i].position;
                    lvr.LineColor = Color.white;
                    _mixVisualRequest.AddRequest(lvr);
                    _lines.Add(lvr);
                }
            }
            VisualizerManager.Instance.Register(_mixVisualRequest);
        }

        private void Update()
        {
            for (int i = 0; i < points.Count; i++)
            {
                _points[i].PointPos = points[i].position;
                if (i > 0)
                {
                    _lines[i - 1].StartPos = points[i - 1].position;
                    _lines[i - 1].EndPos = points[i].position;
                }
            }
        }
    }
}