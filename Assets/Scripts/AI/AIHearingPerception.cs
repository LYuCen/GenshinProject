using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI
{
    public class AIHearingPerception :  MonoBehaviour
    {
        public Transform player;
        public float radius;
        public float angle;

        private SectorChecker _sectorChecker;
        private CircleVisualRequest _circleVisualRequest; // 圆形绘制请求
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

            _circleVisualRequest = new CircleVisualRequest();

            _circleVisualRequest.Radius = radius;
            _circleVisualRequest.Color = Color.yellow;

            _pointVisualRequest = new PointVisualRequest();
            _pointVisualRequest.PointColor = Color.gray;
            _pointVisualRequest.Enable = false;

            _mixVisualRequest = new MixVisualRequest();
            _mixVisualRequest.AddRequest(_circleVisualRequest);
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

            _circleVisualRequest.Center = transform.position;

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