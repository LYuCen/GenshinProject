using System.Collections.Generic;
using AI.VisualRequest.VisualRequest;
using mSystem.EQSSystem;
using mSystem.EQSSystem.EqsContext;
using mSystem.EQSSystem.EqsGenerator;
using mSystem.EQSSystem.EqsTest;
using UnityEngine;

namespace AI.EqsController
{
    public class CircleEqsControllerTest : MonoBehaviour
    {
        public Transform target;
        private IEqsSystem _eqsSystem;
        private CircleEqsGenerator _eqsGenerator;
        private List<IEqsTest> _eqsTests = new List<IEqsTest>();

        private IReadOnlyList<EqsPoint> _points;
        private BaseContext _context;
        
        [Header("圆半径")] public float CircleRadius = 10f;       // 网格宽度
        [Header("高度偏移量")] public float HeightOffset = 0f; // 高度偏移量
        [Header("点数量")] public int PointCount = 20; // 点数目
        [Header("投射高度")] public float RayOriginHeight = 10f; // 投射高度
        [Header("半径")] public float Radius = 2f;
        [Header("圆形坐标")] public Vector3 center;
        public EqsPoint BasePoint;
        public bool enable;

        private MixVisualRequest _mvr;
        private List<PointVisualRequest> _list = new List<PointVisualRequest>();

        private InCircleTest _inCircleTest;
        private InCircleTest _inCircleTest2;

        private void Start()
        {
            _eqsGenerator = new CircleEqsGenerator();
            _eqsGenerator.Center = target.position;
            _eqsGenerator.Radius = CircleRadius;
            _eqsGenerator.HeightOffset = HeightOffset;
            _eqsGenerator.PointCount = PointCount;
            _eqsGenerator.RayOriginHeight = RayOriginHeight;

            _context = new BaseContext(transform, target);

            _inCircleTest = new InCircleTest(transform.position, 2f);
            _inCircleTest2 = new InCircleTest(transform.position, 3f, 1, true);
            
            _eqsTests.Add(new NavMeshReachableTest());
            _eqsTests.Add(new SightTargetTest(1f, false, LayerMask.GetMask("Wall")));
            _eqsTests.Add(new DistanceToPositionTest(transform.position));
            _eqsTests.Add(_inCircleTest);
            _eqsTests.Add(_inCircleTest2);
            _eqsSystem = new EqsSystem(_eqsGenerator, _eqsTests);
            _eqsSystem.RunQuery(_context);
            _points = _eqsSystem.Points;

            _mvr = new MixVisualRequest();
            for (int i = 0; i < _points.Count; i++)
            {
                PointVisualRequest pvr = new PointVisualRequest();
                pvr.PointColor = _points[i].IsValid ? Color.green : Color.red;
                pvr.PointPos = _points[i].Position;
                pvr.Text = _points[i].Score.ToString("F2");
                _list.Add(pvr);
                _mvr.AddRequest(pvr);
            }

            BasePoint = _eqsSystem.GetBestPoint();
            for (int i = 0; i < _points.Count; i++)
            {
                if(_points[i] == BasePoint) _list[i].PointColor = Color.cyan;
            }
            
            
            VisualizerManager.Instance.Register(_mvr);
        }
        
        

        private void Update()
        {
            _eqsSystem.RunQuery(_context);
            
            _mvr.Enable = enable;
            if (!enable) return;
            
            _eqsGenerator.Center = center;
            
            _inCircleTest.Radius = Radius;
            _inCircleTest.Center = transform.position;
            _inCircleTest2.Center = transform.position;
            _eqsGenerator.Radius = CircleRadius;
            _eqsGenerator.HeightOffset = HeightOffset;
            _eqsGenerator.PointCount = PointCount;
            _eqsGenerator.RayOriginHeight = RayOriginHeight;
            
            _points = _eqsSystem.Points;
            if (_points.Count == _list.Count)
            {
                for (int i = 0; i < _points.Count; i++)
                {
                    _list[i].PointColor = _points[i].IsValid ? Color.green : Color.red;
                    _list[i].PointPos = _points[i].Position;
                    _list[i].Text = _points[i].Score.ToString("F2");
                }
                
                BasePoint = _eqsSystem.GetBestPoint();
                for (int i = 0; i < _points.Count; i++)
                {
                    if(_points[i] == BasePoint) _list[i].PointColor = Color.cyan;
                }
            }
            else
            {
                _mvr.ClearRequest();
                for (int i = 0; i < _points.Count; i++)
                {
                    PointVisualRequest pvr = new PointVisualRequest();
                    pvr.PointColor = _points[i].IsValid ? Color.green : Color.red;
                    pvr.PointPos = _points[i].Position;
                    pvr.Text = _points[i].Score.ToString("F2");
                    _list.Add(pvr);
                    _mvr.AddRequest(pvr);
                }
                BasePoint = _eqsSystem.GetBestPoint();
                for (int i = 0; i < _points.Count; i++)
                {
                    if(_points[i] == BasePoint) _list[i].PointColor = Color.cyan;
                }
            }
        }
    }
}