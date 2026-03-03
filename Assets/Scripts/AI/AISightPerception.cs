using AI.VisualRequest;
using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI
{
    public class AISightPerception : MonoBehaviour
    {
        public Transform player;
        public float radius = 10f;
        public float angle = 90f;

        private SectorChecker _sectorChecker;
        private SectorVisualRequest _sectorVisualRequest; // 扇形绘制请求
        private PointVisualRequest _pointVisualRequest;   // 点绘制请求
        private MixVisualRequest _mixVisualRequest;
        
        private bool _isRegisterPoint;

        private void Awake()
        {
            _sectorChecker = new SectorChecker
            {
                Radius = radius,
                Angle = angle
            };

            _sectorVisualRequest = new SectorVisualRequest();

            _sectorVisualRequest.Radius = radius;
            _sectorVisualRequest.Angle = angle;

            _pointVisualRequest = new PointVisualRequest();
            _pointVisualRequest.Enable = false;

            _mixVisualRequest = new MixVisualRequest();
            _mixVisualRequest.AddRequest(_sectorVisualRequest);
            _mixVisualRequest.AddRequest(_pointVisualRequest);
        }

        private void OnEnable()
        {
            if (VisualizerManager.Instance)
            {
                VisualizerManager.Instance.Register(_mixVisualRequest);
            }
        }

        private void Update()
        {
            if (!player) return;

            _sectorVisualRequest.Origin = transform.position;
            _sectorVisualRequest.Forward = transform.forward;

            // 判断玩家是否在扇形内
            bool inside = _sectorChecker.IsInside(transform.position, transform.forward, player.position);
            _pointVisualRequest.PointPos = player.position;
            _pointVisualRequest.Enable = inside;

        }

        private void OnDisable()
        {
            if (VisualizerManager.Instance)
            {
                VisualizerManager.Instance.Unregister(_mixVisualRequest);
            }
        }
    }
}