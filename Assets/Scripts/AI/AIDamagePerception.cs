using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI
{
    public class AIDamagePerception : MonoBehaviour
    {
        public Transform player;
        private PointVisualRequest _request;

        private void Awake()
        {
            _request = new PointVisualRequest();
            _request.PointColor = Color.red;
        }

        private void OnEnable()
        {
            if (VisualizerManager.Instance)
            {
                VisualizerManager.Instance.Register(_request);
            }
        }

        private void Update()
        {
            _request.PointPos = player.position;
        }
        
        private void OnDisable()
        {
            if (VisualizerManager.Instance)
            {
                VisualizerManager.Instance.Unregister(_request);
            }
        }
    }
}