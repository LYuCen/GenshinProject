using AI.VisualRequest.VisualRequest;
using UnityEngine;

namespace AI
{
    public class AISightPerception : MonoBehaviour
    {
        public Transform player;
        public float radius = 10f;
        public float angle = 90f;
        public float sightTime = 3f;
        public bool isSight;
        public Vector3 sightPoint;
        public bool sightTarget;
        public bool playerCanSee;

        private SectorChecker _sectorChecker;
        private SectorVisualRequest _sectorVisualRequest; // 扇形绘制请求
        private PointVisualRequest _pointVisualRequest;   // 点绘制请求
        private MixVisualRequest _mixVisualRequest;
        
        private bool _isRegisterPoint;
        private float _time;

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
            _pointVisualRequest.PointColor = Color.green;

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

            Vector3 v3 = new Vector3(0, 1.5f, 0);
            _sectorVisualRequest.Origin = transform.position;
            _sectorVisualRequest.Forward = transform.forward;

            // 判断玩家是否在扇形内
            bool inside = _sectorChecker.IsInside(transform.position, transform.forward, player.position);
            
            if (inside && !Physics.Linecast(transform.position + v3, player.position + v3, LayerMask.GetMask("Wall")))
            {
                _pointVisualRequest.PointPos = player.position;
                sightPoint = player.position;
                _pointVisualRequest.Enable = true;
                _time = sightTime;
                _pointVisualRequest.Text = _time.ToString("F2");
                isSight = true;
                sightTarget = true;
            }
            else
            {
                sightTarget = false;
                _time = Mathf.Max(_time - Time.deltaTime, 0f);
                if (_time > 0) _pointVisualRequest.Text = _time.ToString("F2");
                else
                {
                    _pointVisualRequest.Enable = false;
                    isSight = false;
                }
            }

            playerCanSee = !Physics.Linecast(transform.position + v3, player.position + v3, LayerMask.GetMask("Wall"));
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